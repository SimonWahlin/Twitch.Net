using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Optional;
using Optional.Unsafe;
using Twitch.Net.EventSub.Converter;
using Twitch.Net.EventSub.Events;
using Twitch.Net.EventSub.Models;
using Twitch.Net.EventSub.Notifications;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.EventSub
{
    public class EventSubService : IEventSubService
    {
        private readonly ILogger<EventSubService> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly ITokenResolver _tokenResolver;
        private readonly EventSubConfig _config;
        private readonly EventSubEventHandler _eventHandler;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public EventSubService(
            ILogger<EventSubService> logger,
            IHttpClientFactory factory,
            IOptions<EventSubConfig> config,
            ITokenResolver tokenResolver
            )
        {
            _eventHandler = new EventSubEventHandler(logger);
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                IncludeFields = true,
                Converters =
                {
                    new ConditionConverter()
                }
            };
            
            _logger = logger;
            _factory = factory;
            _tokenResolver = tokenResolver;
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
            var model = GetModel(raw, type);

            if (!model.HasValue)
                _logger.LogDebug($"Unhandled event {type} with version {version}");
            else
                _eventHandler.InvokeNotification(model.ValueOrFailure(), type);
        }

        private Option<INotificationEvent> GetModel(
            string raw,
            string @event
            ) => @event switch
            {
                EventSubTypes.ChannelFollow => ConvertDataToModel<ChannelFollowNotificationEvent>(raw),
                EventSubTypes.ChannelUpdate => ConvertDataToModel<ChannelUpdateNotificationEvent>(raw),
                EventSubTypes.ChannelSubscription => ConvertDataToModel<ChannelSubscribeNotificationEvent>(raw),
                EventSubTypes.ChannelSubscriptionEnd => ConvertDataToModel<ChannelSubscribeEndNotificationEvent>(raw),
                EventSubTypes.ChannelSubscriptionGift => ConvertDataToModel<ChannelSubscribeGiftNotificationEvent>(raw),
                EventSubTypes.ChannelSubscriptionMessage => ConvertDataToModel<ChannelSubscribeMessageNotificationEvent>(raw),
                EventSubTypes.ChannelCheer => ConvertDataToModel<ChannelCheerNotificationEvent>(raw),
                EventSubTypes.ChannelRaid => ConvertDataToModel<ChannelRaidNotificationEvent>(raw),
                EventSubTypes.ChannelBan => ConvertDataToModel<ChannelBanNotificationEvent>(raw),
                EventSubTypes.ChannelUnban => ConvertDataToModel<ChannelUnbanNotificationEvent>(raw),
                EventSubTypes.ChannelModeratorAdd => ConvertDataToModel<ChannelModeratorAddNotificationEvent>(raw),
                EventSubTypes.ChannelModeratorRemove => ConvertDataToModel<ChannelModeratorRemoveNotificationEvent>(raw),
                EventSubTypes.ChannelCustomRewardAdd => ConvertDataToModel<ChannelRedeemChangeNotificationEvent>(raw),
                EventSubTypes.ChannelCustomRewardUpdate => ConvertDataToModel<ChannelRedeemChangeNotificationEvent>(raw),
                EventSubTypes.ChannelCustomRewardRemove => ConvertDataToModel<ChannelRedeemChangeNotificationEvent>(raw),
                EventSubTypes.ChannelRedemptionAdd => ConvertDataToModel<ChannelRedeemRedemptionNotificationEvent>(raw),
                EventSubTypes.ChannelRedemptionUpdate => ConvertDataToModel<ChannelRedeemRedemptionNotificationEvent>(raw),
                EventSubTypes.ChannelPollBegin => ConvertDataToModel<ChannelPollBeginNotificationEvent>(raw),
                EventSubTypes.ChannelPollProgress => ConvertDataToModel<ChannelPollProgressNotificationEvent>(raw),
                EventSubTypes.ChannelPollEnd => ConvertDataToModel<ChannelPollEndNotificationEvent>(raw),
                EventSubTypes.ChannelPredictBegin => ConvertDataToModel<ChannelPredictBeginNotificationEvent>(raw),
                EventSubTypes.ChannelPredictProgress => ConvertDataToModel<ChannelPredictProgressNotificationEvent>(raw),
                EventSubTypes.ChannelPredictLock => ConvertDataToModel<ChannelPredictLockNotificationEvent>(raw),
                EventSubTypes.ChannelPredictEnd => ConvertDataToModel<ChannelPredictEndNotificationEvent>(raw),
                EventSubTypes.ExtensionBitTransaction => ConvertDataToModel<ExtensionBitTransactionNotificationEvent>(raw),
                EventSubTypes.ChannelHypeTrainBegin => ConvertDataToModel<ChannelHypeTrainBeginNotificationEvent>(raw),
                EventSubTypes.ChannelHypeTrainProgress => ConvertDataToModel<ChannelHypeTrainProgressNotificationEvent>(raw),
                EventSubTypes.ChannelHypeTrainEnd => ConvertDataToModel<ChannelHypeTrainEndNotificationEvent>(raw),
                EventSubTypes.StreamOnline => ConvertDataToModel<StreamOnlineNotificationEvent>(raw),
                EventSubTypes.StreamOffline => ConvertDataToModel<StreamOfflineNotificationEvent>(raw),
                EventSubTypes.UserAuthRevoke => ConvertDataToModel<UserAuthRevokeNotificationEvent>(raw),
                EventSubTypes.UserUpdate => ConvertDataToModel<UserUpdateNotificationEvent>(raw),
                _ => Option.None<INotificationEvent>()
            };

        private Option<INotificationEvent> ConvertDataToModel<T>(string raw)
        {
            try
            {
                return JsonSerializer.Deserialize<NotificationEvent<T>>(raw, _jsonSerializerOptions)!.Some<INotificationEvent>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex. Message);
                return Option.None<INotificationEvent>();
            }
        }
        
        public async Task<SubscribeResult> Subscribe(SubscribeModel model, string? token = null)
        {
            try
            {
                var client = _factory.CreateClient(EventSubServiceFactory.EventSubFactory);
                var request = new HttpRequestMessage(HttpMethod.Post, "/helix/eventsub/subscriptions");

                var bytes = JsonSerializer.SerializeToUtf8Bytes(model, _jsonSerializerOptions);
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
                var request = new HttpRequestMessage(HttpMethod.Get, $"/helix/eventsub/subscriptions");

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
}