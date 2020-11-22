using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Twitch.Net.Communication.Clients
{
    public interface IClientListener
    {
        Task OnReconnected();
        Task OnMessage(WebSocketMessageType messageType, string message);
        Task OnConnected();
        Task OnDisconnected();
    }
}