namespace Twitch.Net.Utils.Configurations
{
    public class TwitchCredentialConfiguration
    {
        public string Username { get; init; }
        public string OAuth { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string BaseChannel { get; init; } // does not really have to do with credentials - but is nice to have as "basic" startup
    }
}