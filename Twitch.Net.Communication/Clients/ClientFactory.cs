using Twitch.Net.Shared.Logger;

namespace Twitch.Net.Communication.Clients
{
    public static class ClientFactory
    {
        public static IClient CreateClient(IClientListener clientListener, string address, IConnectionLogger clientLogger = null)
        {
            return new WebSocketClient(clientListener, address, clientLogger);
        }
    }
}