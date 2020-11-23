using System;
using System.Threading;
using System.Threading.Tasks;
using RateLimiter;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Client.Handlers.Join
{
    internal class JoinQueueHandler
    {
        private CancellationTokenSource _tokenSource;
        private readonly IrcClient _client;
        private readonly TimeLimiter _rateLimiter;

        public JoinQueueHandler(IrcClient client, bool verified)
        {
            _tokenSource = new CancellationTokenSource();
            _client = client;
            _rateLimiter = TimeLimiter.GetFromMaxCountByInterval(
                verified ? 2000 : 20, // rate limits from docs
                new TimeSpan(10));
        }

        public void Enqueue(ChatChannel channel) =>
            Task.Run(() => // fire and forget
            {
                if (!_client.IsConnected)
                {
                    channel.SetConnectionState(ChatChannelConnectionState.Failure);
                    return; // if we are not connected we wont even try
                }

                channel.SetConnectionState(ChatChannelConnectionState.Queued);
                var result = _rateLimiter.Enqueue(() => PerformJoin(channel), _tokenSource.Token);
                if (result.IsCanceled || result.IsFaulted)
                    channel.SetConnectionState(ChatChannelConnectionState.Failure);
            });

        private void PerformJoin(ChatChannel channel)
        {
            channel.SetConnectionState(ChatChannelConnectionState.Connecting);
            _client.SendRaw(Rfc2812Parser.Join($"#{channel.ChannelName.ToLower()}"));
            Task.Run(async () =>
            {
                // if the status of the channel does not update from connecting to connected within 10 seconds
                // then we will just assume it failed to join the channel ( Connected state is set by MessageHandler )
                await Task.Delay(10000);
                if (channel.ConnectionState == ChatChannelConnectionState.Connecting)
                    channel.SetConnectionState(ChatChannelConnectionState.Failure);
            });
        }

        public void Reset()
        {
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
        }
    }
}