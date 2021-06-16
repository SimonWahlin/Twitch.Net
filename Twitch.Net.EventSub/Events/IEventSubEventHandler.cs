using System;
using System.Threading.Tasks;
using Twitch.Net.EventSub.Notifications;

namespace Twitch.Net.EventSub.Events
{
    public interface IEventSubEventHandler
    {
        // Notifications
        event Func<NotificationEvent<ChannelFollowNotificationEvent>, Task> OnFollowed;
        event Func<NotificationEvent<ChannelUpdateNotificationEvent>, Task> OnChannelUpdate;
    }
}