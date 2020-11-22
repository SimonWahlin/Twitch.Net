using System;
using System.Threading.Tasks;

namespace Twitch.Net.Client.Client
{
    public interface IIrcClientEventHandler
    {
        // Connection
        event Func<Task> OnPubSubConnected;
        event Func<Task> OnPubSubReconnect;
        event Func<Task> OnPubSubDisconnect;
    }
}