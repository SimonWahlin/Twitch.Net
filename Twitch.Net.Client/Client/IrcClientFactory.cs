using Twitch.Net.Shared.Configurations;

namespace Twitch.Net.Client.Client
{
    public static class IrcClientFactory
    {
        public static IIrcClient CreateClient(
            IIrcClientCredentialConfiguration credentialConfiguration, 
            bool useSsl = true
            )
            => new IrcClient(
                credentialConfiguration,
                useSsl);
    }
}