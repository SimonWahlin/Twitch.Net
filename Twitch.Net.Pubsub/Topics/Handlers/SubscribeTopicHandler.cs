using System.Text.Json;
using System.Threading.Tasks;
using Twitch.Net.PubSub.Client.Handlers.Events;
using Twitch.Net.PubSub.Events;

namespace Twitch.Net.PubSub.Topics.Handlers
{
    internal class SubscribeTopicHandler : ITopicHandler
    {
        public async Task<bool> Handle(IPubSubClientEventInvoker eventInvoker, ParsedTopicMessage message)
        {
            var data = JsonSerializer.Deserialize<SubscribeEvent>(message.JsonData);
            
            if (data.Data.Message.Gifted)
                await eventInvoker.InvokeGiftedSubscriptionEventTopic(data.Data.Message);
            else
                await eventInvoker.InvokeSubscriptionEventTopic(data.Data.Message);
            
            return true;
        }
    }
}