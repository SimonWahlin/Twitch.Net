namespace Twitch.Net.PubSub.Client
{
    public static class PubSubClientFactory
    {
        public static IPubSubClient CreateClient(bool useSsl = true) 
            => new PubSubClient(useSsl);
    }
}