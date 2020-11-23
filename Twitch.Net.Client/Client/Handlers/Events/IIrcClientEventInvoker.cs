using System.Threading.Tasks;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public interface IIrcClientEventInvoker
    {
        // Connection
        Task InvokeOnPubSubConnected();
        Task InvokeOnPubSubReconnect();
        Task InvokeOnPubSubDisconnect();
    }
}