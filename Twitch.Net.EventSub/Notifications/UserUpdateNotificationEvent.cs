using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "user_id": "1337",
     * "user_login": "cool_user",
     * "user_name": "Cool_User",
     * "email": "user@email.com",  // Requires user:read:email scope
     * "description": "cool description"
     */
    public class UserUpdateNotificationEvent
    {
        [JsonPropertyName("user_id")]
        public string UserIdString { get; init; }
        public int UserId => int.Parse(UserIdString);
        
        [JsonPropertyName("user_login")]
        public string UserLogin { get; init; }
        
        [JsonPropertyName("user_name")]
        public string UserName { get; init; }
        
        [JsonPropertyName("email")]
        public string Email { get; init; }
        
        [JsonPropertyName("description")]
        public string Description { get; init; }

        public bool ContainsEmail => !string.IsNullOrEmpty(Email);
    }
}