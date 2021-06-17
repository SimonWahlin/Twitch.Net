using System;
using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "id": "1234",
     * "broadcaster_user_id": "1337",
     * "broadcaster_user_login": "cool_user",
     * "broadcaster_user_name": "Cool_User",
     * "user_id": "9001",
     * "user_login": "cooler_user",
     * "user_name": "Cooler_User",
     * "user_input": "pogchamp",
     * "status": "unfulfilled", // unfulfilled, fulfilled or cancelled
     * "reward": {
     *      "id": "9001",
     *      "title": "title",
     *      "cost": 100,
     *      "prompt": "reward prompt"
     * },
     * "redeemed_at": "2020-07-15T17:16:03.17106713Z"
     */
    public class ChannelRedeemRedemptionNotificationEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }
        
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterIdString { get; init; }
        public int BroadcasterId => int.Parse(BroadcasterIdString);
        
        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; init; }
        
        [JsonPropertyName("user_id")]
        public string UserIdString { get; init; }
        public int UserId => int.Parse(UserIdString);
        
        [JsonPropertyName("user_login")]
        public string UserLogin { get; init; }
        
        [JsonPropertyName("user_name")]
        public string UserName { get; init; }
        
        [JsonPropertyName("user_input")]
        public string UserInput { get; init; }
        
        [JsonPropertyName("status")]
        public string StatusString { get; init; }
        public ChannelRedeemStatus Status => StatusString.ToRedeemStatus();
        
        [JsonPropertyName("reward")]
        public string Reward { get; init; }
        
        [JsonPropertyName("redeemed_at")]
        public DateTime RedeemedAt { get; init; }
    }

    /**
     * "id": "9001",
     * "title": "title",
     * "cost": 100,
     * "prompt": "reward prompt"
     */
    public class ChannelRedeemRedemptionReward
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }
        
        [JsonPropertyName("title")]
        public string Title { get; init; }
        
        [JsonPropertyName("cost")]
        public int Cost { get; init; }
        
        [JsonPropertyName("prompt")]
        public string Prompt { get; init; }
    }

    public enum ChannelRedeemStatus
    {
        Unknown,
        Unfulfilled,
        Fulfilled,
        Cancelled
    }
    
    public static class ChannelRedeemStatusExtension
    {
        // to lower only in-case the event sub ever changes..
        public static ChannelRedeemStatus ToRedeemStatus(this string value) => value.ToLower() switch
        {
            "unfulfilled" => ChannelRedeemStatus.Unfulfilled,
            "fulfilled" => ChannelRedeemStatus.Fulfilled,
            "cancelled" => ChannelRedeemStatus.Cancelled,
            _ => ChannelRedeemStatus.Unknown
        };
    }
}