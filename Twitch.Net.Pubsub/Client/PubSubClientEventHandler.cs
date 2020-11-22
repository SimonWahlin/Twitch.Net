using System;
using System.Threading.Tasks;
using Twitch.Net.PubSub.Events;
using Twitch.Net.Utils.Extensions;

namespace Twitch.Net.PubSub.Client
{
    /**
     * We are doing this it make "PubSubClient" not overflowed with a bunch of logic & methods
     * So we can make it clearer of what is what, instead of nesting it all in a single class
     */
    public class PubSubClientEventHandler : IPubSubClientEventHandler, IPubSubClientEventInvoker
    {
        #region Listeners
        
        // Connections
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
        
        // Response message
        private readonly AsyncEvent<Func<MessageResponse, Task>> _responseMessageEvents = new();
        public event Func<MessageResponse, Task> OnResponseMessageEvent 
        {
            add => _responseMessageEvents.Add(value);
            remove => _responseMessageEvents.Remove(value);
        }
        
        // Unhandled
        private readonly AsyncEvent<Func<UnknownMessageEvent, Task>> _unknownMessageEvents = new();
        public event Func<UnknownMessageEvent, Task> OnUnknownMessageEvent 
        {
            add => _unknownMessageEvents.Add(value);
            remove => _unknownMessageEvents.Remove(value);
        }
        
        // Redeems
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customRedeemEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemEvent 
        {
            add => _customRedeemEvents.Add(value);
            remove => _customRedeemEvents.Remove(value);
        }
                
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customRedeemCreatedEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemCreated 
        {
            add => _customRedeemCreatedEvents.Add(value);
            remove => _customRedeemCreatedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customRedeemUpdatedEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemUpdated 
        {
            add => _customRedeemUpdatedEvents.Add(value);
            remove => _customRedeemUpdatedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customRedeemDeletedEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemDeleted 
        {
            add => _customRedeemDeletedEvents.Add(value);
            remove => _customRedeemDeletedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customRedeemStatusUpdateEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemStatusUpdate 
        {
            add => _customRedeemStatusUpdateEvents.Add(value);
            remove => _customRedeemStatusUpdateEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customProgressStartedEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemProgressStarted 
        {
            add => _customProgressStartedEvents.Add(value);
            remove => _customProgressStartedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customProgressFinishedEvents = new();
        public event Func<CommunityPointsEvent, Task> OnCustomRedeemProgressFinished 
        {
            add => _customProgressFinishedEvents.Add(value);
            remove => _customProgressFinishedEvents.Remove(value);
        }
        
        private readonly AsyncEvent<Func<CommunityPointsEvent, Task>> _customAutomaticUpdatedEvents = new();
        public event Func<CommunityPointsEvent, Task> OnAutomaticRedeemUpdated 
        {
            add => _customAutomaticUpdatedEvents.Add(value);
            remove => _customAutomaticUpdatedEvents.Remove(value);
        }
        
        #endregion

        #region Invokers
        
        // Connections
        public async Task InvokeOnPubSubConnected()
            => await _connectedEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnPubSubReconnect()
            => await _reconnectEvents.InvokeAsync().ConfigureAwait(false);

        public async Task InvokeOnPubSubDisconnect()
            => await _disconnectEvents.InvokeAsync().ConfigureAwait(false);
        
        // Message
        public async Task InvokeResponseMessage(MessageResponse arg)
            => await _responseMessageEvents.InvokeAsync(arg).ConfigureAwait(false);

        // Unhandled
        public async Task InvokeUnknownMessage(UnknownMessageEvent arg)
            => await _unknownMessageEvents.InvokeAsync(arg).ConfigureAwait(false);
        
        // Redeems
        public async Task InvokeRedeemTopic(CommunityPointsEvent arg) 
            => await _customRedeemEvents.InvokeAsync(arg).ConfigureAwait(false);
        
        public async Task InvokeCustomRedeemCreatedTopic(CommunityPointsEvent arg) 
            => await _customRedeemCreatedEvents.InvokeAsync(arg).ConfigureAwait(false);

        public async Task InvokeCustomRedeemUpdatedTopic(CommunityPointsEvent arg)
            => await _customRedeemUpdatedEvents.InvokeAsync(arg).ConfigureAwait(false);

        public async Task InvokeCustomRedeemDeletedTopic(CommunityPointsEvent arg)
            => await _customRedeemDeletedEvents.InvokeAsync(arg).ConfigureAwait(false);

        public async Task InvokeCustomRedeemStatusUpdateTopic(CommunityPointsEvent arg)
            => await _customRedeemStatusUpdateEvents.InvokeAsync(arg).ConfigureAwait(false);

        public async Task InvokeCustomRedeemInProgressTopic(CommunityPointsEvent arg)
            => await _customProgressStartedEvents.InvokeAsync(arg).ConfigureAwait(false);

        public async Task InvokeCustomRedeemFinishedProgressTopic(CommunityPointsEvent arg)
            => await _customProgressFinishedEvents.InvokeAsync(arg).ConfigureAwait(false);

        public async Task InvokeAutomaticRedeemUpdatedTopic(CommunityPointsEvent arg)
            => await _customAutomaticUpdatedEvents.InvokeAsync(arg).ConfigureAwait(false);
        
        #endregion
    }
}