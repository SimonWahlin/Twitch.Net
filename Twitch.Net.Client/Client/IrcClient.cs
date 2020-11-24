using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Client.Handlers.Join;
using Twitch.Net.Client.Events;
using Twitch.Net.Client.Events.Handlers;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;
using Twitch.Net.Communication.Clients;
using Twitch.Net.Communication.Events;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.RateLimits;

namespace Twitch.Net.Client.Client
{
    internal class IrcClient : IIrcClient, IClientListener
    {
        private readonly IClient _connectionClient;
        private readonly UserAccountStatus _userAccountStatus;
        private readonly IIrcClientCredentialConfiguration _credentialConfiguration;
        private readonly IrcClientEventHandler _eventHandler;
        private readonly JoinQueueHandler _joinChannelHandler;
        private readonly List<ChatChannel> _channels = new();
        
        // Event handlers
        private readonly JoinEventHandler _joinEventHandler;
        private readonly LeftEventHandler _leftEventHandler;
        private readonly MessageEventHandler _messageEventHandler;

        public IrcClient(
            IIrcClientCredentialConfiguration credentialConfiguration, 
            IClient connectionClient,
            UserAccountStatus userAccountStatus = null
            )
        {
            _credentialConfiguration = credentialConfiguration;
            _userAccountStatus = userAccountStatus ?? new UserAccountStatus();

            // Setup client handlers
            _eventHandler = new IrcClientEventHandler();
            _joinChannelHandler = new JoinQueueHandler(this, _userAccountStatus.IsVerifiedBot);
            
            // Setup event handlers
            _joinEventHandler = new JoinEventHandler(this);
            _leftEventHandler = new LeftEventHandler(this, channel => _channels.Remove(channel));
            _messageEventHandler = new MessageEventHandler();
            
            // Create connection
            _connectionClient = connectionClient;
            connectionClient.SetListener(this);
        }

        public IReadOnlyList<ChatChannel> Channels
            => _channels;

        public IIrcClientEventHandler Events
            => _eventHandler;

        public bool IsConnected
            => _connectionClient.IsConnected;

        public async Task<bool> ConnectAsync() =>
            await _connectionClient.ConnectAsync();

        public bool SendMessage(string channel, string message)
            => SendMessage(
                _channels.FirstOrDefault(x => x.ChannelName.Equals(channel, StringComparison.OrdinalIgnoreCase)), 
                message);

        public bool SendMessage(ChatChannel chatChannel, string message)
        {
            if (string.IsNullOrEmpty(message) || 
                chatChannel == null || 
                string.IsNullOrEmpty(_credentialConfiguration?.Username))
                return false;
            
            if (message.Length > 500)
            {
                // TODO : IRC Logger of some kind
                return false;
            }

            var outbound = new OutboundChatMessage
            {
                Channel = chatChannel.ChannelName,
                Message = message,
                Username = _credentialConfiguration.Username
            };

            return _connectionClient.Send(outbound.ToString()); // this can return false if connection is closed.
        }

        public bool SendRaw(string message)
            => _connectionClient.Send(message);

        public ChatChannel JoinChannel(string channel)
        {
            if (string.IsNullOrEmpty(channel)) // just to prevent someone from even try this.
                return new ChatChannel {ChannelName = channel}.SetConnectionState(ChatChannelConnectionState.Failure);
            
            if (_channels.Any(x => x.ChannelName.Equals(channel, StringComparison.OrdinalIgnoreCase)))
                return _channels.First(x => x.ChannelName.Equals(channel, StringComparison.OrdinalIgnoreCase));

            var chat = new ChatChannel
            {
                ChannelName = channel
            };

            if (!_connectionClient.IsConnected)
                return chat.SetConnectionState(ChatChannelConnectionState.Failure);

            _channels.Add(chat);
            _joinChannelHandler.Enqueue(chat, () => // on failure we wanna set the status and remove it from the list
            {
                chat.SetConnectionState(ChatChannelConnectionState.Failure);
                _channels.Remove(chat);
            });
            
            return chat;
        }

        /**
         * Authenticate with twitch IRC server as specified : https://dev.twitch.tv/docs/irc/guide
         */
        private void AuthenticateConnection()
        {
            _connectionClient.Send(Rfc2812Parser.Pass(_credentialConfiguration.OAuth));
            _connectionClient.Send(Rfc2812Parser.Nick(_credentialConfiguration.Username));

            _connectionClient.Send("CAP REQ twitch.tv/membership");
            _connectionClient.Send("CAP REQ twitch.tv/commands");
            _connectionClient.Send("CAP REQ twitch.tv/tags");
        }
        
        public async Task OnConnected() 
        {
            AuthenticateConnection();
            await _eventHandler.InvokeOnIrcConnected();
        }

        public async Task OnReconnected()
        {
            AuthenticateConnection();
            await _eventHandler.InvokeOnIrcReconnect();
        }

        public async Task OnDisconnected(ClientDisconnected clientDisconnected)
        {
            // Channel joining / joined needs to be reset since we lost connection
            _joinChannelHandler.Reset();
            _channels.Clear();
            
            await _eventHandler.InvokeOnIrcDisconnect(clientDisconnected);
        }
        
        private const string MessageStringSeparator = "\r\n";
        public async Task OnMessage(WebSocketMessageType messageType, string messages)
        {
            if (messageType == WebSocketMessageType.Close) // if server sends "close" message we reconnect
                await _connectionClient.ReconnectAsync();
            else if (messageType == WebSocketMessageType.Text) // if it is text, then we parse & handle it
            {
                var lines = messages.Split(MessageStringSeparator);
                foreach (var line in lines.Where(l => l.Length > 1)) // one line equals one message
                    await OnHandleMessage(line);
            } 
        }

        private async Task OnHandleMessage(string message)
        {
            var parsed = RawIrcMessageParser.ParseRawIrcMessage(message);
            
            // if we do not define what command type, we can disconnect a random bot by typing it in the chat
            if (parsed.Message.Contains("Login authentication failed") && parsed.Command == IrcCommand.Notice) 
            {
                await _connectionClient.DisconnectAsync("Login authentication failed");
                return;
            }

            var handled = parsed.Command switch
            {
                IrcCommand.Join => await _joinEventHandler.Handle(_eventHandler, parsed),
                _ => false
            };

            if (!handled)
            {
                // if anyone wants to know what the output data was
                // and easier if you wanna implement a missing feature too
                await _eventHandler.InvokeOnUnknownMessage(new UnknownMessageEvent
                {
                    Parsed = parsed,
                    Raw = message
                });
            }
        }
    }
}