using Twitch.Net.Communication.Clients;

namespace Twitch.Net.PubSub.Client
{
    public static class PubSubClientFactory
    {
        public static IPubSubClient CreateClient(IClient connectionClient = null) => 
            new PubSubClient(
                connectionClient ?? ClientFactory.CreateClient(PubSubClientAddressBuilder.CreateAddress())
                );
    }
}