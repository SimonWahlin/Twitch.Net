using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Twitch.Net.EventSub.Notifications;
using Twitch.Net.Shared.Extensions;

namespace Twitch.Net.EventSub.Events
{
    public class EventSubEventHandler : IEventSubEventHandler, IEventSubEventInvoker
    {
        private readonly ILogger<EventSubService> _logger;

        public EventSubEventHandler(ILogger<EventSubService> logger)
        {
            _logger = logger;
        }
        
        #region Listeners 

        private readonly AsyncEvent<Func<NotificationEvent<ChannelFollowNotificationEvent>, Task>> _followedEvents = new();
        public event Func<NotificationEvent<ChannelFollowNotificationEvent>, Task> OnFollowed
        {
            add => _followedEvents.Add(value);
            remove => _followedEvents.Remove(value);
        }

        private readonly AsyncEvent<Func<NotificationEvent<ChannelUpdateNotificationEvent>, Task>> _channelUpdateEvents = new();
        public event Func<NotificationEvent<ChannelUpdateNotificationEvent>, Task> OnChannelUpdate
        {
            add => _channelUpdateEvents.Add(value);
            remove => _channelUpdateEvents.Remove(value);
        }

        #endregion

        #region Invokers

        public async Task InvokeNotification(INotificationEvent @event)
        {
            if (@event is NotificationEvent<ChannelFollowNotificationEvent> follow)
                await _followedEvents.InvokeAsync(follow).ConfigureAwait(false);
            else if (@event is NotificationEvent<ChannelUpdateNotificationEvent> chlUpdate)
                await _channelUpdateEvents.InvokeAsync(chlUpdate).ConfigureAwait(false);
            else
                _logger.LogError($"Event of type {@event} does not have an invoker event implemented.");
        }

        #endregion
    }
}