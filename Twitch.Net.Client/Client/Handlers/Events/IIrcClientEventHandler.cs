using System;
using System.Threading.Tasks;
using Twitch.Net.Client.Events;
using Twitch.Net.Communication.Events;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public interface IIrcClientEventHandler
    {
        // Connection
        event Func<Task> OnIrcConnected;
        event Func<Task> OnIrcReconnect;
        event Func<ClientDisconnected, Task> OnIrcDisconnect;
        
        // Unknown
        event Func<UnknownMessageEvent, Task> OnUnknownMessage;
        
        // Chat messages
        event Func<ChatMessageEvent, Task> OnChatMessage;
        event Func<BeingHostedEvent, Task> OnBeingHosted;
        
        // Channel join/leave/update
        event Func<JoinedChannelEvent, Task> OnUserJoinedChannel;
        event Func<LeftChannelEvent, Task> OnUserLeftChannel;
        event Func<JoinedChannelEvent, Task> OnJoinedChannel;
        event Func<LeftChannelEvent, Task> OnLeftChannel;
        event Func<ChannelStateUpdateEvent, Task> OnChannelStateUpdate;
    }
}