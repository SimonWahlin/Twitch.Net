using Microsoft.Extensions.Logging;

namespace Twitch.Net.Communication.Clients
{
    public class WebSocketClientFactory : IClientFactory
    {
        private readonly ILogger<IClient> _logger;

        public WebSocketClientFactory(ILogger<IClient> logger)
        {
            _logger = logger;
        }

        public IClient CreateClient(string address = null) =>
            new WebSocketClient(address, _logger);
    }
}