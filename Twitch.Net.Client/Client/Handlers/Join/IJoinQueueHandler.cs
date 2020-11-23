using System.Collections.Generic;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Client.Handlers.Join
{
    public interface IJoinQueueHandler
    {
        IReadOnlyList<ChatChannel> InQueue { get; }
    }
}