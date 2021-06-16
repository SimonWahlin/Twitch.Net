using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Twitch.Net.EventSub;
using Twitch.Net.EventSub.Notifications;

namespace Twitch.Net.Sample.EventSubServer
{
    public class DummyService : IHostedService
    {
        private readonly IEventSubService _eventSubService;

        public DummyService(IEventSubService eventSubService)
        {
            _eventSubService = eventSubService;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _eventSubService.Events.OnFollowed += EventOnFollowed;
            return Task.CompletedTask;
        }

        private Task EventOnFollowed(NotificationEvent<ChannelFollowNotificationEvent> arg)
        {
            Console.WriteLine($"[UserFollowedEvent] {arg.Event.UserName}#{arg.Event.UserId} followed {arg.Event.BroadcasterUserName}#{arg.Event.BroadcasterId}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}