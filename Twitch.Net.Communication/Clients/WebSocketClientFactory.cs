using Microsoft.Extensions.Logging;

namespace Twitch.Net.Communication.Clients
{
    public class WebSocketClientFactory : IClientFactory
    {
        public IClient CreateClient<T>(ILogger<T> logger, string address = null) =>
            new WebSocketClient<T>(logger, address);
    }
}