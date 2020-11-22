using System.Threading.Tasks;
using Twitch.Net.PubSub.Topics;
using Twitch.Net.Shared.Logger;

namespace Twitch.Net.PubSub.Client
{
    public interface IPubSubClient
    {
        IConnectionLoggerConfiguration ConnectionLoggerConfiguration { get; }
        Task<bool> ConnectAsync();
        TopicBuilder CreateBuilder();
        IPubSubClientEventHandler Events { get; }
    }
}