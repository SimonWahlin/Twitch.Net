﻿using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "from_broadcaster_user_id": "1234",
     * "from_broadcaster_user_login": "cool_user",
     * "from_broadcaster_user_name": "Cool_User",
     * "to_broadcaster_user_id": "1337",
     * "to_broadcaster_user_login": "cooler_user",
     * "to_broadcaster_user_name": "Cooler_User",
     * "viewers": 9001
     */
    public class ChannelRaidNotificationEvent
    {
        [JsonPropertyName("from_broadcaster_user_id")]
        public string FromBroadcasterUserIdString { get; init; }
        public int FromBroadcasterUserId => int.Parse(FromBroadcasterUserIdString);
        
        [JsonPropertyName("from_broadcaster_user_login")]
        public string FromBroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("from_broadcaster_user_name")]
        public string FromBroadcasterUserName { get; init; }
        
        [JsonPropertyName("to_broadcaster_user_id")]
        public string ToBroadcasterUserIdString { get; init; }
        public int ToBroadcasterUserId => int.Parse(ToBroadcasterUserIdString);
        
        [JsonPropertyName("to_broadcaster_user_login")]
        public string ToBroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("to_broadcaster_user_name")]
        public string ToBroadcasterUserName { get; init; }
        
        [JsonPropertyName("viewers")]
        public int Viewers { get; init; }
    }
}