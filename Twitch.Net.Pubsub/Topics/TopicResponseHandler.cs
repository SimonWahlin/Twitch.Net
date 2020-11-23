using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Twitch.Net.PubSub.Client;
using Twitch.Net.Pubsub.Client.Handlers.Events;
using Twitch.Net.PubSub.Events;
using Twitch.Net.PubSub.Topics.Handlers;

namespace Twitch.Net.PubSub.Topics
{
    public class TopicResponseHandler
    {
        private readonly IPubSubClientEventInvoker _eventInvoker;
        private readonly ITopicHandler _redeemTopicHandler = new RedeemTopicHandler();

        public TopicResponseHandler(IPubSubClientEventInvoker eventInvoker)
        {
            _eventInvoker = eventInvoker;
        }

        public async Task<bool> Handle(string type, Dictionary<string, object> parsed) =>
            type switch
            {
                "response" => await HandleResponseMessage(parsed),
                "message" => await HandleMessageTopic(ParseMessage(parsed)),
                _ => false
            };

        private ParsedTopicMessage ParseMessage(Dictionary<string, object> parsed)
        {
            if (parsed.ContainsKey("data") && parsed["data"] is JsonElement { ValueKind: JsonValueKind.Object } element) 
            {
                try
                {
                    var topic = element.GetProperty("topic").GetString()?.Split(".")[0];
                    var data = element.GetProperty("message").GetString();
                    
                    if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(data))
                        return new ParsedTopicMessage
                        {
                            Parsed = true,
                            Topic = topic,
                            JsonData = data
                        };
                } catch { /* Will be sent as a "UnknownMessageEvent" if it failed */ }
            }
            return new ParsedTopicMessage();
        }

        private async Task<bool> HandleMessageTopic(ParsedTopicMessage message)
        {
            if (!message.Parsed) // so if the parsing went wrong, we will return false to trigger "UnknownMessageEvent"
                return false;
            
            return message.Topic switch
            {
                "community-points-channel-v1" => await _redeemTopicHandler.Handle(_eventInvoker, message),
                _ => false
            };
        }

        private async Task<bool> HandleResponseMessage(Dictionary<string, object> parsed)
        {
            if (parsed.ContainsKey("nonce") && parsed.ContainsKey("error"))
            {
                await _eventInvoker.InvokeResponseMessage(new MessageResponse
                {
                    Nonce = parsed["nonce"].ToString(),
                    Error = parsed["error"].ToString()
                });
                return true;
            }
            
            return false;
        }
    }
}