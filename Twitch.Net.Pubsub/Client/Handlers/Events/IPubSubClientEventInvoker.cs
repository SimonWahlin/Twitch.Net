using System.Threading.Tasks;
using Twitch.Net.Communication.Events;
using Twitch.Net.PubSub.Events;

namespace Twitch.Net.PubSub.Client.Handlers.Events
{
    public interface IPubSubClientEventInvoker
    {
        // Connection
        Task InvokeOnPubSubConnected();
        Task InvokeOnPubSubReconnect();
        Task InvokeOnPubSubDisconnect(ClientDisconnected clientDisconnected);
        
        // Response message
        Task InvokeResponseMessage(MessageResponse arg);
        
        // Unhandled
        Task InvokeUnknownMessage(UnknownMessageEvent arg);

        // Redeems - possible to improve by having interfaces behind the CommunityPointsEvent for better type-safe of each event
        Task InvokeRedeemTopic(CommunityPointsEvent arg);
        Task InvokeCustomRedeemCreatedTopic(CommunityPointsEvent arg);
        Task InvokeCustomRedeemUpdatedTopic(CommunityPointsEvent arg);
        Task InvokeCustomRedeemDeletedTopic(CommunityPointsEvent arg);
        Task InvokeCustomRedeemStatusUpdateTopic(CommunityPointsEvent arg);
        Task InvokeCustomRedeemInProgressTopic(CommunityPointsEvent arg);
        Task InvokeCustomRedeemFinishedProgressTopic(CommunityPointsEvent arg);
        Task InvokeAutomaticRedeemUpdatedTopic(CommunityPointsEvent arg);
    }
}