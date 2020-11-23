namespace Twitch.Net.Shared.Configurations
{
    public class TwitchCredentialConfiguration : 
        IIrcClientCredentialConfiguration,
        IPubSubCredentialConfiguration,
        IApiCredentialConfiguration,
        IUserAccountResolverCredentialConfiguration
    {
        public string Username { get; init; }
        public string OAuth { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string BaseChannel { get; init; } // does not really have to do with credentials - but is nice to have as "basic" startup
    }
}