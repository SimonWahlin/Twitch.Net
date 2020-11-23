using Twitch.Net.Communication.Clients;

namespace Twitch.Net.PubSub.Client
{
    public static class PubSubClientFactory
    {
        public static IPubSubClient CreateClient(IClient connectionClient) 
            => new PubSubClient(connectionClient);
    }
}