using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;
using Twitch.Net.Communication.Clients;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Logger;

namespace Twitch.Net.Client.Client
{
    internal class IrcClient : IIrcClient, IClientListener
    {
        private const string IrcServerAddress = "irc-ws.chat.twitch.tv";
        private readonly IClient _connectionClient;
        private readonly IIrcClientCredentialConfiguration _credentialConfiguration;
        private readonly ConnectionLogger _connectionLogger;
        private readonly IrcClientEventHandler _eventHandler;
        private readonly List<ChatChannel> _channels = new();
        private readonly ConcurrentQueue<ChatChannel> _joinQueue = new();
        
        public IrcClient(IIrcClientCredentialConfiguration credentialConfiguration, bool useSsl)
        {
            _credentialConfiguration = credentialConfiguration;
            _eventHandler = new IrcClientEventHandler();
            _connectionLogger = new ConnectionLogger();
            _connectionClient = ClientFactory.CreateClient(
                this,
                $"{Protocol(useSsl)}://{IrcServerAddress}:{Port(useSsl)}",
                _connectionLogger
            );
        }

        private string Protocol(bool ssl) => ssl ? "wss" : "ws";
        private string Port(bool ssl) => ssl ? "443" : "80";

        public IReadOnlyList<ChatChannel> Channels { get; }

        public IConnectionLoggerConfiguration ConnectionLoggerConfiguration
            => _connectionLogger;

        public IIrcClientEventHandler Events
            => _eventHandler;

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
            
            _joinQueue.Enqueue(chat.SetConnectionState(ChatChannelConnectionState.Queued));
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
            await _eventHandler.InvokeOnPubSubConnected();
        }

        public async Task OnReconnected()
        {
            AuthenticateConnection();
            await _eventHandler.InvokeOnPubSubReconnect();
        }

        public async Task OnDisconnected() 
            => await _eventHandler.InvokeOnPubSubDisconnect();

        public async Task OnMessage(WebSocketMessageType messageType, string message)
        {
            if (messageType == WebSocketMessageType.Close) // if server sends "close" message we reconnect
                await _connectionClient.ReconnectAsync();
            else if (messageType == WebSocketMessageType.Text) // if it is text, then we parse & handle it
                await OnHandleMessage(message);
        }

        private Task OnHandleMessage(string message)
        {
            return Task.CompletedTask;
        }
    }
}