namespace Twitch.Net.Shared.Configurations
{
    public interface ITokenResolverCredentialConfiguration
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }
}