using Microsoft.Extensions.Logging;
using Websocket.Client;

namespace Twitch.Net.Communication.Clients
{
    public class WebSocketClientFactory : IClientFactory
    {
        private readonly ILogger<IWebsocketClient> _logger;

        public WebSocketClientFactory(ILogger<IWebsocketClient> logger)
        {
            _logger = logger;
        }

        public IClient CreateClient(string address = null) =>
            new WebSocketClient(address, _logger);
    }
}