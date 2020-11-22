using System.Text.Json;
using System.Threading.Tasks;
using Twitch.Net.PubSub.Client;
using Twitch.Net.PubSub.Events;

namespace Twitch.Net.PubSub.Topics.Handlers
{
    internal class RedeemTopicHandler : ITopicHandler
    {
        public async Task<bool> Handle(IPubSubClientEventInvoker eventInvoker, ParsedTopicMessage message)
        {
            var data = JsonSerializer.Deserialize<CommunityPointsEvent>(message.JsonData);

            if (data?.EventType == CommunityPointsEventType.Redeem)
                await eventInvoker.InvokeRedeemTopic(data);
            else if (data?.EventType == CommunityPointsEventType.CustomCreated)
                await eventInvoker.InvokeCustomRedeemCreatedTopic(data);
            else if (data?.EventType == CommunityPointsEventType.CustomDeleted)
                await eventInvoker.InvokeCustomRedeemDeletedTopic(data);
            else if (data?.EventType == CommunityPointsEventType.CustomUpdated)
                await eventInvoker.InvokeCustomRedeemUpdatedTopic(data);
            else if (data?.EventType == CommunityPointsEventType.AutomaticUpdated)
                await eventInvoker.InvokeAutomaticRedeemUpdatedTopic(data);
            else if (data?.EventType == CommunityPointsEventType.InProgress)
                await eventInvoker.InvokeCustomRedeemInProgressTopic(data);
            else if (data?.EventType == CommunityPointsEventType.ProgressFinished)
                await eventInvoker.InvokeCustomRedeemFinishedProgressTopic(data);
            else if (data?.EventType == CommunityPointsEventType.RedemptionUpdate)
                await eventInvoker.InvokeCustomRedeemStatusUpdateTopic(data);
            else
                return false; // if it is an unknown event type we did not "handle"
            
            return true;
        }
    }
}