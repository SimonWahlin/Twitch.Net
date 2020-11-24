using System;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Net.Client.Client;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Events.Handlers
{
    public class JoinEventHandler : IHandler
    {
        private readonly IIrcClient _client;

        public JoinEventHandler(IIrcClient client)
        {
            _client = client;
        }
        
        public async Task<bool> Handle(IIrcClientEventInvoker eventInvoker, IrcMessage message)
        {
            if (!string.IsNullOrEmpty(message.Channel) && !string.IsNullOrEmpty(message.User))
            {
                var channel = _client.Channels.FirstOrDefault(
                    c => c.ChannelName.Equals(message.Channel, StringComparison.OrdinalIgnoreCase));
                if (channel != null)
                {
                    channel.SetConnectionState(ChatChannelConnectionState.Connected);
                    await eventInvoker.InvokeOnChannelJoined(new JoinedChannelEvent
                    {
                        BotUsername = message.User,
                        Channel = channel
                    });
                }
            }
            else
                return false;
            
            return true;
        }
    }
}