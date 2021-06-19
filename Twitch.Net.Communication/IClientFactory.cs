using Twitch.Net.Communication.Clients;

namespace Twitch.Net.Communication
{
    public interface IClientFactory
    {
        IClient CreateClient(string address = null);
    }
}