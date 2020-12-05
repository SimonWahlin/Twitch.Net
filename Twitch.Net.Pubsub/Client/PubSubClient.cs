﻿using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using Twitch.Net.Communication.Clients;
using Twitch.Net.Communication.Events;
using Twitch.Net.Communication.Models;
using Twitch.Net.PubSub.Client.Handlers.Events;
using Twitch.Net.PubSub.Events;
using Twitch.Net.PubSub.Topics;
using Twitch.Net.Shared.Extensions;

namespace Twitch.Net.PubSub.Client
{
    public class PubSubClient : IPubSubClient, IClientListener
    {
        private readonly Random _gen = new();
        private readonly IClient _connectionClient;
        private readonly ILogger<PubSubClient> _logger;
        private readonly TopicResponseHandler _topicResponseHandler;
        private readonly PubSubClientEventHandler _eventHandler;

        private readonly Timer _pingTimer;
        private readonly Timer _pongTimer;
        private readonly string _pingJson;

        public PubSubClient(IClient connectionClient, ILogger<PubSubClient> logger = null)
        {
            _eventHandler = new PubSubClientEventHandler();
            _topicResponseHandler = new TopicResponseHandler(_eventHandler);
            _connectionClient = connectionClient;
            _logger = logger;
            connectionClient.SetListener(this);

            // Ping handler setup
            _pingTimer = new Timer
            {
                Interval = TimeSpan.FromSeconds(_gen.Next(120, 280)).TotalMilliseconds
            };
            _pingTimer.Elapsed += PingTickHandler;
            _pingJson = JsonSerializerHelper.SerializeModel(new PingModel());
            _pingTimer.Start(); // we can just start it and let it continues run.
            
            // Pong handler setup 
            _pongTimer = new Timer
            {
                Interval = TimeSpan.FromSeconds(10).TotalMilliseconds
            };
            _pongTimer.Elapsed += PongTickHandler;
        }

        public TopicBuilder CreateBuilder() 
            => new(this);

        public IPubSubClientEventHandler Events
            => _eventHandler;

        public bool IsConnected
            => _connectionClient.IsConnected;

        public Task<bool> ConnectAsync() 
            => _connectionClient.ConnectAsync();

        public void Send(string data)
            => _connectionClient.Send(data);

        public async Task OnMessage(WebSocketMessageType messageType, string message)
        {
            if (messageType == WebSocketMessageType.Close) // if server sends "close" message we reconnect
                await _connectionClient.ReconnectAsync();
            else if (messageType == WebSocketMessageType.Text) // if it is text, then we parse & handle it
                await OnHandleMessage(message);
        }

        private async Task OnHandleMessage(string message)
        {
            var parsed = JsonSerializerHelper.Deserialize(message);
            if (!parsed.ContainsKey("type")) // if there is no type in the message, we ignore it.
                return;

            var type = parsed["type"].ToString()?.ToLower();

            var handled = type switch
            {
                "pong" => HandlePong(),
                "reconnect" => await HandleReconnect(),
                _ => await _topicResponseHandler.Handle(type, parsed)
            };

            if (!handled)
            {
                // if anyone wants to know what the output data was
                // and easier if you wanna implement a missing feature too
                await _eventHandler.InvokeUnknownMessage(new UnknownMessageEvent
                {
                    Data = parsed,
                    Raw = message
                });
            }
        }

        private async Task<bool> HandleReconnect()
        {
            _logger?.LogInformation("Server requested reconnection - Performing reconnect.");
            await _connectionClient.ReconnectAsync();
            return true;
        }

        private bool HandlePong()
        {
            _logger?.LogTrace("PONG");
            _pongTimer.Stop();
            return true;
        }

        public async Task OnConnected()
            => await _eventHandler.InvokeOnPubSubConnected();

        public async Task OnReconnected()
        {
            await _eventHandler.InvokeOnPubSubReconnect();
            await _eventHandler.InvokeOnPubSubConnected();
        }

        public async Task OnDisconnected(ClientDisconnected clientDisconnected) 
            => await _eventHandler.InvokeOnPubSubDisconnect(clientDisconnected);

        private void PingTickHandler(object sender, ElapsedEventArgs e)
        {
            if (!_connectionClient.IsConnected)
                return; // if we are not connected, we do not wanna send ping
            
            // The reason to why we are changing the interval is because the documentation recommends to do "jitter"
            // And instead we are just making it do ping random within the interval of 2 & 4 minutes
            // If client does not send ping each "5 minutes" the server will disconnect the client
            _pingTimer.Stop(); // to change the interval we have to "stop" > "change" > "start"
            _pingTimer.Interval = TimeSpan.FromSeconds(_gen.Next(2*60, 4*60)).TotalMilliseconds;
            _pingTimer.Start();
            
            _logger?.LogTrace("Sending ping to the server");
            _pongTimer.Start();
            Send(_pingJson);
        }

        private void PongTickHandler(object sender, ElapsedEventArgs e)
        {
            _logger?.LogInformation("Failed responding to pong - Performing reconnect.");
            Task.Run(async () => await _connectionClient.ReconnectAsync());
            _pongTimer.Stop();
        }
    }
}