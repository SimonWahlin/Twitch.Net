using System.Threading.Tasks;

namespace Twitch.Net.Client.Client
{
    public interface IIrcClientEventInvoker
    {
        // Connection
        Task InvokeOnPubSubConnected();
        Task InvokeOnPubSubReconnect();
        Task InvokeOnPubSubDisconnect();
    }
}