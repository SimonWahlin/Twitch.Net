using System;
using System.Threading.Tasks;
using Twitch.Net.Client.Events;
using Twitch.Net.Communication.Events;
using Twitch.Net.Shared.Extensions;

namespace Twitch.Net.Client.Client.Handlers.Events
{
    public class IrcClientEventHandler : IIrcClientEventHandler, IIrcClientEventInvoker
    {
        #region Listeners
        
        // Connections - TODO Add connection args (since IRC chat)
        private readonly AsyncEvent<Func<Task>> _connectedEvents = new();
        public event Func<Task> OnIrcConnected 
        {
            add => _connectedEvents.Add(value);
            remove => _connectedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<Task>> _reconnectEvents = new();
        public event Func<Task> OnIrcReconnect 
        {
            add => _reconnectEvents.Add(value);
            remove => _reconnectEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<ClientDisconnected, Task>> _disconnectEvents = new();
        public event Func<ClientDisconnected, Task> OnIrcDisconnect 
        {
            add => _disconnectEvents.Add(value);
            remove => _disconnectEvents.Remove(value);
        }
        
        // Unknown
        private readonly AsyncEvent<Func<UnknownMessageEvent, Task>> _unknownMessageEvents = new();
        public event Func<UnknownMessageEvent, Task> OnUnknownMessage 
        {
            add => _unknownMessageEvents.Add(value);
            remove => _unknownMessageEvents.Remove(value);
        }
        
        // Chat messages
        private readonly AsyncEvent<Func<ChatMessageEvent, Task>> _chatMessageEvents = new();
        public event Func<ChatMessageEvent, Task> OnChatMessage 
        {
            add => _chatMessageEvents.Add(value);
            remove => _chatMessageEvents.Remove(value);
        }


        // Channel join/leave/update
        private readonly AsyncEvent<Func<JoinedChannelEvent, Task>> _joinChannelEvents = new();
        public event Func<JoinedChannelEvent, Task> OnJoinedChannel 
        {
            add => _joinChannelEvents.Add(value);
            remove => _joinChannelEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<LeftChannelEvent, Task>> _leftChannelEvents = new();
        public event Func<LeftChannelEvent, Task> OnLeftChannel 
        {
            add => _leftChannelEvents.Add(value);
            remove => _leftChannelEvents.Remove(value);
        }
        
        #endregion
        
        #region Invokers

        // Connections
        public async Task InvokeOnIrcConnected()
            => await _connectedEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnIrcReconnect()
            => await _reconnectEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnIrcDisconnect(ClientDisconnected clientDisconnected)
            => await _disconnectEvents.InvokeAsync(clientDisconnected).ConfigureAwait(false);

        // Unknown
        public async Task InvokeOnUnknownMessage(UnknownMessageEvent messageEvent)
            => await _unknownMessageEvents.InvokeAsync(messageEvent).ConfigureAwait(false);
        
        // Chat messages
        public async Task InvokeOnMessage(ChatMessageEvent chatMessageEvent)
            => await _chatMessageEvents.InvokeAsync(chatMessageEvent).ConfigureAwait(false);
        
        // Channel join/leave/update
        public async Task InvokeOnChannelJoined(JoinedChannelEvent joinedChannelEvent)
            => await _joinChannelEvents.InvokeAsync(joinedChannelEvent).ConfigureAwait(false); 
        
        public async Task InvokeOnChannelLeft(LeftChannelEvent leftChannelEvent)
            => await _leftChannelEvents.InvokeAsync(leftChannelEvent).ConfigureAwait(false); 

        #endregion
    }
}