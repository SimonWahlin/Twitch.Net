using System.Threading.Tasks;
using Twitch.Net.Client.Events;
using Twitch.Net.Communication.Events;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public interface IIrcClientEventInvoker
    {
        // Connection
        Task InvokeOnIrcConnected();
        Task InvokeOnIrcReconnect();
        Task InvokeOnIrcDisconnect(ClientDisconnected clientDisconnected);
        
        // Unknown
        Task InvokeOnUnknownMessage(UnknownMessageEvent messageEvent);
        
        // Chat messages
        Task InvokeOnMessage(ChatMessageEvent chatMessageEvent);
        Task InvokeOnBeingHosted(BeingHostedEvent beingHostedEvent);
        
        // Channel join/leave/update
        Task InvokeOnUserJoinedChannel(JoinedChannelEvent joinedChannelEvent);
        Task InvokeOnUserLeftChannel(LeftChannelEvent leftChannelEvent);
        Task InvokeOnChannelJoined(JoinedChannelEvent joinedChannelEvent);
        Task InvokeOnChannelLeft(LeftChannelEvent leftChannelEvent);
        Task InvokeOnChannelStateUpdate(ChannelStateUpdateEvent channelStateUpdateEvent);
    }
}