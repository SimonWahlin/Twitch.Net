using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Client.Handlers.Join
{
    internal class JoinQueueHandler : IJoinQueueHandler
    {
        private readonly ConcurrentQueue<ChatChannel> _joinQueue = new();
        private readonly IrcClient _client;

        public JoinQueueHandler(IrcClient client)
        {
            _client = client;
        }

        public IReadOnlyList<ChatChannel> InQueue
            => _joinQueue.ToList();

        public void Enqueue(ChatChannel channel)
            => _joinQueue.Enqueue(channel);

        public void Clear()
            => _joinQueue.Clear();
    }
}