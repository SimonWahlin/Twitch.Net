namespace Twitch.Net.Shared.Configurations
{
    public interface IUserAccountResolverCredentialConfiguration
    {
        public string Username { get; init; }
        public string OAuth { get; init; }
        public string ClientId { get; init; }
    }
}