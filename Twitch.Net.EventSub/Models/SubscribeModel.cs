using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Models
{
    public class SubscribeModel
    {
        [JsonPropertyName("type")]
        public string Type { get; init; }
        
        [JsonPropertyName("version")]
        public string Version { get; init; }
        
        [JsonPropertyName("condition")]
        public SubscribeConditionModel Condition { get; init; }
        
        [JsonPropertyName("transport")]
        public SubscribeTransportModel Transport { get; init; }
    }

    public class SubscribeConditionModel
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string Broadcaster { get; init; }
    }

    public class SubscribeTransportModel
    {
        [JsonPropertyName("method")]
        public string Method { get; init; }
        
        [JsonPropertyName("callback")]
        public string Callback { get; init; }
        
        [JsonPropertyName("secret")]
        public string Secret { get; init; }
    }
}