using System;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Net.Client.Client;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Events.Handlers
{
    public class LeftEventHandler : IHandler
    {
        private readonly IIrcClient _client;
        private readonly Action<ChatChannel> _removeAction;

        public LeftEventHandler(
            IIrcClient client,
            Action<ChatChannel> removeAction)
        {
            _client = client;
            _removeAction = removeAction;
        }
        
        public async Task<bool> Handle(IIrcClientEventInvoker eventInvoker, IrcMessage message)
        {
            if (!string.IsNullOrEmpty(message.Channel) && !string.IsNullOrEmpty(message.User))
            {
                var channel = _client.Channels.FirstOrDefault(c =>
                    c.ChannelName.Equals(message.Channel, StringComparison.OrdinalIgnoreCase));
                if (channel != null)
                {
                    channel.SetConnectionState(ChatChannelConnectionState.Left);
                    _removeAction(channel);
                    await eventInvoker.InvokeOnChannelLeft(new LeftChannelEvent
                    {
                        BotUsername = message.User,
                        Channel = message.Channel
                    });
                }
            }
            else
                return false;
            
            return true;
        }
    }
}