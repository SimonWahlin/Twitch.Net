using System;
using System.Threading.Tasks;
using Twitch.Net.PubSub.Events;

namespace Twitch.Net.Pubsub.Client.Handlers.Events
{
    public interface IPubSubClientEventHandler
    {
        // Connection
        event Func<Task> OnPubSubConnected;
        event Func<Task> OnPubSubReconnect;
        event Func<Task> OnPubSubDisconnect;
        
        // Response message
        event Func<MessageResponse, Task> OnResponseMessageEvent;
        
        /**
         * Can also fire if a parsing went wrong or unhandled
         */
        event Func<UnknownMessageEvent, Task> OnUnknownMessageEvent;
        
        // Redeems
        event Func<CommunityPointsEvent, Task> OnCustomRedeemEvent;
        event Func<CommunityPointsEvent, Task> OnCustomRedeemCreated;
        event Func<CommunityPointsEvent, Task> OnCustomRedeemUpdated;
        event Func<CommunityPointsEvent, Task> OnCustomRedeemDeleted;
        event Func<CommunityPointsEvent, Task> OnAutomaticRedeemUpdated;
        event Func<CommunityPointsEvent, Task> OnCustomRedeemStatusUpdate;
        event Func<CommunityPointsEvent, Task> OnCustomRedeemProgressStarted;
        event Func<CommunityPointsEvent, Task> OnCustomRedeemProgressFinished;
    }
}