namespace Twitch.Net.Shared.Configurations
{
    public interface IChatClientCredentialConfiguration
    {
        public string Username { get; init; }
        public string OAuth { get; init; }
        public string BaseChannel { get; init; } // does not really have to do with credentials - but is nice to have as "basic" startup
    }
}