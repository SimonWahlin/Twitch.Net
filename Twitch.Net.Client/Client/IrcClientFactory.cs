using Twitch.Net.Communication.Clients;
using Twitch.Net.Shared.Configurations;

namespace Twitch.Net.Client.Client
{
    public static class IrcClientFactory
    {
        public static IIrcClient CreateClient(
            IIrcClientCredentialConfiguration credentialConfiguration, 
            IClient connectionClient
            )
            => new IrcClient(
                credentialConfiguration,
                connectionClient);
    }
}