using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "user_id": "1234",
     * "user_login": "cool_user",
     * "user_name": "Cool_User",
     * "broadcaster_user_id": "1337",
     * "broadcaster_user_login": "cooler_user",
     * "broadcaster_user_name": "Cooler_User"
     */
    public class ChannelModeratorAddNotificationEvent
    {
        [JsonPropertyName("user_id")]
        public string UserIdString { get; init; }
        public int UserId => int.Parse(UserIdString);
        
        [JsonPropertyName("user_login")]
        public string UserLogin { get; init; }
        
        [JsonPropertyName("user_name")]
        public string UserName { get; init; }
        
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterIdString { get; init; }
        public int BroadcasterId => int.Parse(BroadcasterIdString);
        
        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; init; }
    }
}