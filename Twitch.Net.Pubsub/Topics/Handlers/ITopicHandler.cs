using System.Threading.Tasks;
using Twitch.Net.PubSub.Client;

namespace Twitch.Net.PubSub.Topics.Handlers
{
    internal interface ITopicHandler
    {
        Task<bool> Handle(IPubSubClientEventInvoker eventInvoker, ParsedTopicMessage message);
    }
}