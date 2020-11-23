using System;
using System.Threading.Tasks;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public interface IIrcClientEventHandler
    {
        // Connection
        event Func<Task> OnPubSubConnected;
        event Func<Task> OnPubSubReconnect;
        event Func<Task> OnPubSubDisconnect;
    }
}