namespace Twitch.Net.Shared.Configurations
{
    public interface IUserAccountResolverCredentialConfiguration : ITokenResolverCredentialConfiguration
    {
        public string Username { get; init; }
    }
}