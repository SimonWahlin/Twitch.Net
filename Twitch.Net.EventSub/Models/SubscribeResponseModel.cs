using System;
using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Models
{
    public class SubscribeResponseModel
    {
        [JsonPropertyName("data")]
        public SubscribeResponseDataModel Data { get; init; }
        
        [JsonPropertyName("total")]
        public int Total { get; init; }
        
        [JsonPropertyName("total_cost")]
        public int TotalCost { get; init; }
        
        [JsonPropertyName("max_total_cost")]
        public int MaxTotalCost { get; init; }
    }

    public class SubscribeResponseDataModel
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
        
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; }
        
        [JsonPropertyName("condition")]
        public SubscribeResponseDataConditionModel Condition { get; init; }
        
        [JsonPropertyName("transport")]
        public SubscribeResponseDataTransportModel Transport { get; init; }
    }

    public class SubscribeResponseDataConditionModel
    {
        [JsonPropertyName("version")]
        public string Version { get; init; }
    }
    
    public class SubscribeResponseDataTransportModel
    {
        [JsonPropertyName("method")]
        public string Method { get; init; }
        
        [JsonPropertyName("callback")]
        public string Callback { get; init; }
    }
}