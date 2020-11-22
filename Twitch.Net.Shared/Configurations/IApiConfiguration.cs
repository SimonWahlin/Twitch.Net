namespace Twitch.Net.Shared.Configurations
{
    public interface IApiCredentialConfiguration
    {
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
    }
}