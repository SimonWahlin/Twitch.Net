using System.Threading.Tasks;
using Twitch.Net.Pubsub.Client.Handlers.Events;
using Twitch.Net.PubSub.Topics;

namespace Twitch.Net.PubSub.Client
{
    public interface IPubSubClient
    {
        Task<bool> ConnectAsync();
        TopicBuilder CreateBuilder();
        IPubSubClientEventHandler Events { get; }
    }
}