using Microsoft.Extensions.Logging;
using Twitch.Net.Communication.Clients;

namespace Twitch.Net.Communication
{
    public interface IClientFactory
    {
        IClient CreateClient<T>(ILogger<T> logger, string address = null);
    }
}