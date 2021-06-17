using System.Text.Json.Serialization;

namespace Twitch.Net.EventSub.Notifications
{
    /**
     * "client_id": "crq72vsaoijkc83xx42hz6i37",
     * "user_id": "1337",
     * "user_login": "cool_user",  // Null if the user no longer exists
     * "user_name": "Cool_User"    // Null if the user no longer exists
     */
    public class UserAuthRevokeNotificationEvent
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; init; }
        
        [JsonPropertyName("user_id")]
        public string UserIdString { get; init; }
        public int UserId => int.Parse(UserIdString);
        
        [JsonPropertyName("user_login")]
        public string UserLogin { get; init; }
        
        [JsonPropertyName("user_name")]
        public string UserName { get; init; }

        /**
         * If user login or name is null, then the account no longer exist (according to docs)
         */
        public bool AccountRemoved => string.IsNullOrEmpty(UserLogin);
    }
}