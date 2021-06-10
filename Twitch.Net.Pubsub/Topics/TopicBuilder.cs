using System;
using System.Collections.Generic;
using System.Linq;
using Twitch.Net.PubSub.Client;
using Twitch.Net.Shared.Extensions;

namespace Twitch.Net.PubSub.Topics
{
    public class TopicBuilder
    {
        private const string ValidRandomCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random Gen = new();
        
        private readonly PubSubClient _client;
        private readonly List<string> _topics = new();

        public TopicBuilder(PubSubClient client)
        {
            _client = client;
        }
        
        private TopicBuilder AddTopic(string topic)
        {
            _topics.Add(topic);
            return this;
        }

        public TopicBuilder CreateChannelBitsCheerTopic(long userId)
            => AddTopic($"channel-bits-events-v2.{userId}");
        
        public TopicBuilder CreateChannelSubscribeEventsTopic(long userId)
            => AddTopic($"channel-subscribe-events-v1.{userId}");
        
        public TopicBuilder CreateChannelPointsRedeemTopic(long userId)
            => AddTopic($"channel-points-channel-v1.{userId}");
        
        public void Listen(string token = null)
            => SendTopics(true, token);

        public void Unlisten(string token = null)
            => SendTopics(true, token);

        private void SendTopics(bool listen = true, string token = null)
        {
            if (_topics.Count == 0)
                return;

            var nonce = GenerateNonce();

            var data = new TopicDataModel
            {
                Topics = _topics,
                Token = token?.Replace("oauth:", "", StringComparison.OrdinalIgnoreCase)
            };

            var model = new TopicModel
            {
                Type = listen ? "LISTEN" : "UNLISTEN",
                Nonce = nonce,
                Data = data
            };

            _client.Send(JsonSerializerHelper.SerializeModel(model));
        }

        private static string GenerateNonce() =>
            new(Enumerable.Repeat(ValidRandomCharacters, 18)
                .Select(s => s[Gen.Next(s.Length)]).ToArray());
    }
}