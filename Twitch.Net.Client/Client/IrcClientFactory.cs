using Twitch.Net.Communication.Clients;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.RateLimits;

namespace Twitch.Net.Client.Client
{
    public static class IrcClientFactory
    {
        public static IIrcClient CreateClient(
            IIrcClientCredentialConfiguration credentialConfiguration, 
            IClient connectionClient = null,
            UserAccountStatus userAccountStatus = null
            )
            => new IrcClient(
                credentialConfiguration,
                connectionClient ?? ClientFactory.CreateClient(IrcClientAddressBuilder.CreateAddress()),
                userAccountStatus
                );
    }
}