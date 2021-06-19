using System;
using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Models
{
    public class  SubscribeCallbackSubscriptionModel
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
        
        [JsonPropertyName("transport")]
        public SubscribeCallbackSubscriptionTransportModel Transport { get; init; }
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }
    }

    public class SubscribeCallbackSubscriptionTransportModel
    {
        [JsonPropertyName("method")]
        public string Method { get; init; }
        
        [JsonPropertyName("callback")]
        public string Callback { get; init; }
    }
}