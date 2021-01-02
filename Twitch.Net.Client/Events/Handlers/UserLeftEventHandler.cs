using System;
using System.Threading.Tasks;
using Optional.Collections;
using Twitch.Net.Client.Client;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Events.Handlers
{
    public class UserLeftEventHandler : IHandler
    {
        private readonly IIrcClient _client;
        private readonly Action<ChatChannel> _onLeftAction;

        public UserLeftEventHandler(IIrcClient client, Action<ChatChannel> onLeftAction)
        {
            _client = client;
            _onLeftAction = onLeftAction;
        }
        
        public Task<bool> Handle(IIrcClientEventInvoker eventInvoker, IrcMessage message)
        {
            if (!string.IsNullOrEmpty(message.Channel) && !string.IsNullOrEmpty(message.User))
            {
                var channel = _client.Channels.FirstOrNone(c =>
                    c.ChannelName.Equals(message.Channel, StringComparison.OrdinalIgnoreCase));
                channel.MatchSome(async chl =>
                {
                    if (message.User.Equals(_client.BotUsername, StringComparison.OrdinalIgnoreCase))
                    {
                        _onLeftAction?.Invoke(chl);
                        await eventInvoker.InvokeOnChannelLeft(new LeftChannelEvent
                        {
                            Username = message.User,
                            Channel = chl.SetConnectionState(ChatChannelConnectionState.Left)
                        });
                    }
                    else
                        await eventInvoker.InvokeOnUserLeftChannel(new LeftChannelEvent
                        {
                            Username = message.User,
                            Channel = chl
                        });
                });
            }
            else
                return Task.FromResult(false);
            
            return Task.FromResult(true);
        }
    }
}