using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "broadcaster_user_id": "1337",
     * "broadcaster_user_login": "cooler_user",
     * "broadcaster_user_name": "Cooler_User"
     */
    public class ChannelOfflineNotificationEvent
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterIdString { get; init; }
        public int BroadcasterId => int.Parse(BroadcasterIdString);
        
        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; init; }
    }
}