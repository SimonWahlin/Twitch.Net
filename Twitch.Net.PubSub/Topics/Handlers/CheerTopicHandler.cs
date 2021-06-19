using System.Text.Json;
using System.Threading.Tasks;
using Twitch.Net.PubSub.Client.Handlers.Events;
using Twitch.Net.PubSub.Events;

namespace Twitch.Net.PubSub.Topics.Handlers
{
    internal class CheerTopicHandler : ITopicHandler
    {
        public async Task<bool> Handle(IPubSubClientEventInvoker eventInvoker, ParsedTopicMessage message)
        {
            var data = JsonSerializer.Deserialize<CheerEvent>(message.JsonData);

            // Both anonymous & none anonymous
            await eventInvoker.InvokeCheerTopic(data);
            
            return true;
        }
    }
}