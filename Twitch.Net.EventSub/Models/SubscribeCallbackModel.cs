using System;
using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Models
{
    public class SubscribeCallbackModel
    {
        [JsonPropertyName("challenge")]
        public string Challenge { get; init; }
        
        [JsonPropertyName("subscription")]
        public SubscribeCallbackSubscriptionModel Subscription { get; init; }
    }
    
    public class SubscribeCallbackSubscriptionModel
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }
        
        [JsonPropertyName("status")]
        public string Status { get; init; }
        
        [JsonPropertyName("type")]
        public string Type { get; init; }
        
        [JsonPropertyName("version")]
        public string Version { get; init; }
        
        [JsonPropertyName("cost")]
        public int Cost { get; init; }
        
        [JsonPropertyName("condition")]
        public SubscribeCallbackSubscriptionConditionModel Condition { get; init; }
        
        [JsonPropertyName("transport")]
        public SubscribeCallbackSubscriptionTransportModel Transport { get; init; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }
    }

    public class SubscribeCallbackSubscriptionConditionModel
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string Broadcaster { get; init; }
    }

    public class SubscribeCallbackSubscriptionTransportModel
    {
        [JsonPropertyName("method")]
        public string Method { get; init; }
        
        [JsonPropertyName("callback")]
        public string Callback { get; init; }
    }
}