using System.Text.Json.Serialization;

namespace Twitch.Net.Utils.Credential
{
    public class TwitchClientCredentialAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; } // in seconds

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}