using System.Threading.Tasks;
using Twitch.Net.EventSub.Notifications;

namespace Twitch.Net.EventSub.Events
{
    public interface IEventSubEventInvoker
    {
        Task InvokeNotification(INotificationEvent @event, string type);
    }
}