using System;
using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * DATA STRUCT
     * "user_id": "1234",
     * "user_login": "cool_user",
     * "user_name": "Cool_User",
     * "broadcaster_user_id": "1337",
     * "broadcaster_user_login": "cooler_user",
     * "broadcaster_user_name": "Cooler_User",
     * "followed_at": "2020-07-15T18:16:11.17106713Z"
     */
    public class ChannelFollowNotificationEvent
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; init; }
        
        [JsonPropertyName("user_login")]
        public string UserLogin { get; init; }
        
        [JsonPropertyName("user_name")]
        public string UserName { get; init; }
        
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; init; }
        
        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; init; }
        
        [JsonPropertyName("followed_at")]
        public DateTime FollowedAt { get; init; }
    }
}