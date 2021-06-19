using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Models
{
    public interface IConditionModel
    {
        // empty
    }
    
    public class BroadcasterConditionModel : IConditionModel
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; init; }
    }
    
    public class ToBroadcasterConditionModel : IConditionModel
    {
        [JsonPropertyName("to_broadcaster_user_id")]
        public string BroadcasterId { get; init; }
    }
    
    public class BroadcasterRewardModel : IConditionModel
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; init; }
        
        [JsonPropertyName("reward_id")]
        public string? RewardId { get; init; }
    }
    
    public class ExtensionConditionModel : IConditionModel
    {
        [JsonPropertyName("extension_client_id")]
        public string ClientId { get; init; }
    }
    
    public class ClientConditionModel : IConditionModel
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; init; }
    }
    
    public class UserConditionModel : IConditionModel
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; init; }
    }
}