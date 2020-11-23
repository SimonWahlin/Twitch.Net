using System;
using System.Threading.Tasks;
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
        
        private readonly AsyncEvent<Func<Task>> _disconnectEvents = new();
        public event Func<Task> OnIrcDisconnect 
        {
            add => _disconnectEvents.Add(value);
            remove => _disconnectEvents.Remove(value);
        }
        
        #endregion
        
        #region Invokers

        public async Task InvokeOnIrcConnected()
            => await _connectedEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnIrcReconnect()
            => await _reconnectEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnIrcDisconnect()
            => await _disconnectEvents.InvokeAsync().ConfigureAwait(false);

        #endregion
    }
}