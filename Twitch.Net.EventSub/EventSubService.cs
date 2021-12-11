using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional;
using Optional.Unsafe;
using Twitch.Net.EventSub.Events;
using Twitch.Net.EventSub.Models;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.EventSub;

public class EventSubService : IEventSubService
{
    private readonly ILogger<EventSubService> _logger;
    private readonly IHttpClientFactory _factory;
    private readonly ITokenResolver _tokenResolver;
    private readonly EventSubModelConverter _converter;
    private readonly EventSubConfig _config;
    private readonly EventSubEventHandler _eventHandler;

    public EventSubService(
        ILogger<EventSubService> logger,
        ILoggerFactory loggerFactory,
        IHttpClientFactory factory,
        IOptions<EventSubConfig> config,
        ITokenResolver tokenResolver,
        EventSubModelConverter converter
        )
    {
        _eventHandler = new EventSubEventHandler(loggerFactory.CreateLogger<EventSubEventHandler>());

        _logger = logger;
        _factory = factory;
        _tokenResolver = tokenResolver;
        _converter = converter;
        _config = config.Value;
    }

    public IEventSubEventHandler Events => _eventHandler;

    public async Task<IActionResult> Handle(HttpRequest request)
    {
        try
        {
            var type = request.Headers[EventSubHeaderConst.MessageType].ToString();
            var raw = await request.GetRawBodyStringAsync(); // we need the raw request for "hmac verification"
            if (!HandleVerification(request.Headers, raw, out var verification))
                return new BadRequestResult();
                
            switch (type)
            {
                case "webhook_callback_verification":
                    return new OkObjectResult(verification.Challenge); // HandleVerification handles this indirectly
                case "notification":
                    HandleNotification(request, raw);
                    return new OkResult();
                case "revocation":
                    return new OkResult(); // we do not do anything for now, but at least it is being "handled"
                default:
                    // so we can handle future stuff, in-case twitch add something new we will get an information log about it.
                    _logger.LogInformation($"The event type {type} of EventSub was not being handled");
                    return new BadRequestResult();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new BadRequestResult();
        }
    }

    private bool HandleVerification(IHeaderDictionary headers, string raw, out SubscribeCallbackModel model)
    {
        var hmac =
            headers[EventSubHeaderConst.MessageId] +
            headers[EventSubHeaderConst.MessageTimestamp] +
            raw;
        var sign = $"sha256={GetHash(hmac, _config.SignatureSecret)}";

        model = JsonSerializer.Deserialize<SubscribeCallbackModel>(raw)!;
        return sign == headers[EventSubHeaderConst.MessageSignature];
    }
        
    private static string GetHash(string text, string key)
    {
        var encoding = new UTF8Encoding();

        var textBytes = encoding.GetBytes(text);
        var keyBytes = encoding.GetBytes(key);

        using var hash = new HMACSHA256(keyBytes);
        var hashBytes = hash.ComputeHash(textBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private void HandleNotification(HttpRequest request, string raw)
    {
        var type = request.Headers[EventSubHeaderConst.SubscriptionType].ToString();
        var version = request.Headers[EventSubHeaderConst.SubscriptionVersion].ToString();
        var model = _converter.GetModel(raw, type);

        if (!model.HasValue)
            _logger.LogDebug($"Unhandled event {type} with version {version}");
        else
            _eventHandler.InvokeNotification(model.ValueOrFailure(), type);
    }

    public async Task<SubscribeResult> Subscribe(SubscribeModel model, string? token = null)
    {
        try
        {
            var client = _factory.CreateClient(EventSubServiceFactory.EventSubFactory);
            var request = new HttpRequestMessage(HttpMethod.Post, "/helix/eventsub/subscriptions");

            var bytes = _converter.ConvertModelToBytes(model);
            var content = new ByteArrayContent(bytes);
            content.Headers.Add("Content-Type", "application/json");
            request.Content = content;

            if (string.IsNullOrEmpty(token))
                token = await _tokenResolver.GetToken();
                
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.SendAsync(request);

            if (result.StatusCode == HttpStatusCode.Conflict)
                return new SubscribeResult
                {
                    AlreadyRegistered = true
                };
                
            result.EnsureSuccessStatusCode();

            return new SubscribeResult
            {
                // if it fails it will throw an Exception anyways and not return a null object.
                Output = (await JsonSerializer.DeserializeAsync<SubscribeResponseModel>(
                    await result.Content.ReadAsStreamAsync()
                )).Some()!
            }; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new SubscribeResult();
        }
    }

    public async Task<bool> Unsubscribe(string id, string? token = null)
    {
        try
        {
            var client = _factory.CreateClient(EventSubServiceFactory.EventSubFactory);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/helix/eventsub/subscriptions?id={id}");

            if (string.IsNullOrEmpty(token))
                token = await _tokenResolver.GetToken();
                
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.SendAsync(request);
            result.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task<Option<RegisteredSubscriptions>> Subscriptions(string? token = null)
    {
        try
        {
            var client = _factory.CreateClient(EventSubServiceFactory.EventSubFactory);
            var request = new HttpRequestMessage(HttpMethod.Get, "/helix/eventsub/subscriptions");

            if (string.IsNullOrEmpty(token))
                token = await _tokenResolver.GetToken();
                
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.SendAsync(request);
            result.EnsureSuccessStatusCode();

            return (await JsonSerializer.DeserializeAsync<RegisteredSubscriptions>(await result.Content.ReadAsStreamAsync()))!.Some();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Option.None<RegisteredSubscriptions>();
        }
    }
}