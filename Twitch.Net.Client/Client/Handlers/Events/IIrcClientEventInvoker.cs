using System.Threading.Tasks;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public interface IIrcClientEventInvoker
    {
        // Connection
        Task InvokeOnIrcConnected();
        Task InvokeOnIrcReconnect();
        Task InvokeOnIrcDisconnect();
    }
}