using System.Threading.Tasks;
using Twitch.Net.PubSub.Client.Handlers.Events;

namespace Twitch.Net.PubSub.Topics.Handlers
{
    internal interface ITopicHandler
    {
        Task<bool> Handle(IPubSubClientEventInvoker eventInvoker, ParsedTopicMessage message);
    }
}