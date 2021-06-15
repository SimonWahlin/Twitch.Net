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
        
        #endregion

        #region Invokers

        public async Task InvokeNotification(INotificationEvent @event)
        {
            switch (@event)
            {
                case NotificationEvent<ChannelFollowNotificationEvent> follow:
                    await _followedEvents.InvokeAsync(follow).ConfigureAwait(false);
                    break;
                default:
                    _logger.LogError($"Event of type {@event} does not have an invoker event implemented.");
                    break;
            }
        }

        #endregion
    }
}