using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Twitch.Net.EventSub.Events;
using Twitch.Net.EventSub.Models;

namespace Twitch.Net.EventSub
{
    public interface IEventSubService
    {
        IEventSubEventHandler Events { get; }
        Task<bool> Handle(HttpRequest request);
        Task<bool> Subscribe(SubscribeModel model, string token);
    }
}