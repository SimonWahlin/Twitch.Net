using System;
using System.Threading.Tasks;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public interface IIrcClientEventHandler
    {
        // Connection
        event Func<Task> OnIrcConnected;
        event Func<Task> OnIrcReconnect;
        event Func<Task> OnIrcDisconnect;
    }
}