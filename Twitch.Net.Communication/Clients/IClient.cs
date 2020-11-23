using System.Threading.Tasks;

namespace Twitch.Net.Communication.Clients
{
    public interface IClient
    {
        bool IsConnected { get; }
        Task<bool> ConnectAsync();
        Task<bool> ReconnectAsync();
        bool Send(string data);
        void SetListener(IClientListener listener);
    }
}