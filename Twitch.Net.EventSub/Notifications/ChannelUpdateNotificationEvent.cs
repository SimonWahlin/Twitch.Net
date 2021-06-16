using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "broadcaster_user_id": "1337",
     * "broadcaster_user_login": "cool_user",
     * "broadcaster_user_name": "Cool_User",
     * "title": "Best Stream Ever",
     * "language": "en",
     * "category_id": "21779",
     * "category_name": "Fortnite",
     * "is_mature": false
     */
    public class ChannelUpdateNotificationEvent
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterIdString { get; init; }
        public int BroadcasterId => int.Parse(BroadcasterIdString);
        
        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; init; }
        
        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; init; }
        
        [JsonPropertyName("title")]
        public string Title { get; init; }
        
        [JsonPropertyName("language")]
        public string Language { get; init; }
        
        [JsonPropertyName("category_id")]
        public string CategoryIdString { get; init; }
        public int CategoryId => int.Parse(CategoryIdString);
        
        [JsonPropertyName("category_name")]
        public string CategoryName { get; init; }
        
        [JsonPropertyName("is_mature")]
        public bool IsMature { get; init; }
    }
}