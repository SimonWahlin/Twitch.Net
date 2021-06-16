using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional;
using Optional.Unsafe;
using Twitch.Net.EventSub.Events;
using Twitch.Net.EventSub.Models;
using Twitch.Net.EventSub.Notifications;

namespace Twitch.Net.EventSub
{
    public class EventSubService : IEventSubService
    {
        private readonly ILogger<EventSubService> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly EventSubConfig _config;
        private readonly EventSubEventHandler _eventHandler;

        public EventSubService(
            ILogger<EventSubService> logger,
            IHttpClientFactory factory,
            IOptions<EventSubConfig> config
            )
        {
            _eventHandler = new EventSubEventHandler(logger);
            _logger = logger;
            _factory = factory;
            _config = config.Value;
        }

        public IEventSubEventHandler Events => _eventHandler;

        public async Task<bool> Handle(HttpRequest request)
        {
            try
            {
                var type = request.Headers[EventSubHeaderConst.MessageType].ToString();
                var raw = await request.GetRawBodyStringAsync(); // we need the raw request for "hmac verification"
                if (!HandleVerification(request.Headers, raw))
                    return false;
                
                switch (type)
                {
                    case "webhook_callback_verification":
                        return true; // HandleVerification handles this indirectly
                    case "notification":
                        await HandleNotification(request, raw);
                        return true;
                    case "revocation":
                        return true; // we do not do anything for now, but at least it is being "handled"
                    default:
                        // so we can handle future stuff, in-case twitch add something new we will get an information log about it.
                        _logger.LogInformation($"The event type {type} of EventSub was not being handled");
                        return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private bool HandleVerification(IHeaderDictionary headers, string raw)
        {
            
            var hmac =
                headers[EventSubHeaderConst.MessageId] +
                headers[EventSubHeaderConst.MessageTimestamp] +
                raw;
            var sign = $"sha256={GetHash(hmac, _config.SignatureSecret)}";

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

        private async Task HandleNotification(HttpRequest request, string raw)
        {
            var @event = request.Headers[EventSubHeaderConst.SubscriptionType].ToString();
            var version = request.Headers[EventSubHeaderConst.SubscriptionVersion].ToString();
            var model = GetModel(raw, @event);

            if (!model.HasValue)
                _logger.LogDebug($"Unhandled event {@event} with version {version}");
            else
                await _eventHandler.InvokeNotification(model.ValueOrFailure());
        }

        private Option<INotificationEvent> GetModel(
            string raw,
            string @event
            ) => @event switch
            {
                "channel.follow" => ConvertDataToModel<ChannelFollowNotificationEvent>(raw),
                "channel.update" => ConvertDataToModel<ChannelUpdateNotificationEvent>(raw),
                _ => Option.None<INotificationEvent>()
            };

        private Option<INotificationEvent> ConvertDataToModel<T>(string raw)
        {
            try
            {
                var model = JsonSerializer.Deserialize<NotificationEvent<T>>(raw, new JsonSerializerOptions { IncludeFields = true });
                return model.Some<INotificationEvent>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex. Message);
                return Option.None<INotificationEvent>();
            }
        }
        
        public async Task<bool> Subscribe(SubscribeModel model, string token)
        {
            try
            {
                var client = _factory.CreateClient(EventSubServiceFactory.EventSubFactory);
                var request = new HttpRequestMessage(HttpMethod.Post, "/helix/eventsub/subscriptions");

                var bytes = JsonSerializer.SerializeToUtf8Bytes(model);
                var content = new ByteArrayContent(bytes);
                content.Headers.Add("Content-Type", "application/json");
                request.Content = content;
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
    }
}