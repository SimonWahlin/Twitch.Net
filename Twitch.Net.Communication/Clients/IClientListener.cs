using System.Net.WebSockets;
using System.Threading.Tasks;
using Twitch.Net.Communication.Events;

namespace Twitch.Net.Communication.Clients
{
    public interface IClientListener
    {
        Task OnReconnected();
        Task OnMessage(WebSocketMessageType messageType, string message);
        Task OnConnected();
        Task OnDisconnected(ClientDisconnected clientDisconnected);
    }
}