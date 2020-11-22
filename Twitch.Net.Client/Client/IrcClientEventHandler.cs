using System;
using System.Threading.Tasks;
using Twitch.Net.Shared.Extensions;

namespace Twitch.Net.Client.Client
{
    public class IrcClientEventHandler : IIrcClientEventHandler, IIrcClientEventInvoker
    {
        #region Listeners
        
        // Connections - TODO Add connection args (since IRC chat)
        private readonly AsyncEvent<Func<Task>> _connectedEvents = new();
        public event Func<Task> OnPubSubConnected 
        {
            add => _connectedEvents.Add(value);
            remove => _connectedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<Task>> _reconnectEvents = new();
        public event Func<Task> OnPubSubReconnect 
        {
            add => _reconnectEvents.Add(value);
            remove => _reconnectEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<Task>> _disconnectEvents = new();
        public event Func<Task> OnPubSubDisconnect 
        {
            add => _disconnectEvents.Add(value);
            remove => _disconnectEvents.Remove(value);
        }
        
        #endregion
        
        #region Invokers

        public async Task InvokeOnPubSubConnected()
            => await _connectedEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnPubSubReconnect()
            => await _reconnectEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnPubSubDisconnect()
            => await _disconnectEvents.InvokeAsync().ConfigureAwait(false);

        #endregion
    }
}