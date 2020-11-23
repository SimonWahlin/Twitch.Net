﻿using Microsoft.Extensions.Logging;

namespace Twitch.Net.Communication.Clients
{
    public static class ClientFactory
    {
        public static IClient CreateClient(
            string address = null,
            ILogger<IClient> logger = null
            )
            => new WebSocketClient(address, logger);
    }
}