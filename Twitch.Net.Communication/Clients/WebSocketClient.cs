using System;
using System.Threading.Tasks;
using Twitch.Net.Shared.Logger;
using Websocket.Client;
using Websocket.Client.Models;

namespace Twitch.Net.Communication.Clients
{
    internal class WebSocketClient : IClient
    {
        private readonly IClientListener _clientListener;
        private readonly string _clientAddress;
        private readonly IConnectionLogger _clientLogger;

        private IWebsocketClient _client;
        private bool _reconnecting; // to prevent "faulty" error message

        public bool IsConnected => _client?.IsRunning ?? false;

        public WebSocketClient(
            IClientListener clientListener, 
            string address,
            IConnectionLogger clientLogger = null)
        {
            _clientListener = clientListener;
            _clientAddress = address; // the server address the connection will be connecting to.
            _clientLogger = clientLogger;
        }

        public async Task<bool> ConnectAsync()
        {
            if (_client == null)
            {
                _client = new WebsocketClient(new Uri(_clientAddress))
                {
                    ReconnectTimeout = null // this is to prevent the client from auto disconnecting if we are not getting a response within X time
                };
                _client.ReconnectionHappened.Subscribe(async r => await OnReconnect(r));
                _client.MessageReceived.Subscribe(async m => await OnMessage(m));
                _client.DisconnectionHappened.Subscribe(async d => await Disconnected(d));
            }

            if (IsConnected)
                return true; // we are already connected, no need to connect again
            
            try
            {
                await _client.StartOrFail();
                await Connected(); // trigger event and logger
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ReconnectAsync()
        {
            if (_client == null) // we cannot reconnect before we have done a first connection!
                return false;

            try
            {
                _reconnecting = true;
                
                if (IsConnected)
                    await _client.ReconnectOrFail();
                else
                {
                    await _client.StartOrFail();
                    await OnReconnect(new ReconnectionInfo(ReconnectionType.ByServer));
                }

                _reconnecting = false;
                
                return true;
            }
            catch
            {
                return await ReconnectAsync(); // if reconnect or start fails we just wanna re-try
            }
        }

        private async Task Connected()
        {
            _clientLogger.Log("Connection happened");
            await _clientListener.OnConnected();
        }

        private async Task OnReconnect(ReconnectionInfo reconnectionInfo)
        {
            // we do not want to pass "initial"
            if (reconnectionInfo.Type == ReconnectionType.Initial)
                return;
            
            _clientLogger.Log($"Reconnection happened, type: {reconnectionInfo.Type}");
            await _clientListener.OnReconnected();
        }

        private async Task OnMessage(ResponseMessage responseMessage)
        {
            _clientLogger.MessageLog($"[INCOMING] [Type: {responseMessage.MessageType}] - {responseMessage.Text}");
            await _clientListener.OnMessage(responseMessage.MessageType, responseMessage.Text);
        }

        private async Task Disconnected(DisconnectionInfo disconnectionInfo)
        {
            if (_reconnecting) return; // disconnects are not valid during a reconnection is going on
            
            _clientLogger.MessageLog($"Disconnect happened - Reason: {nameof(disconnectionInfo.Type)}");
            await _clientListener.OnDisconnected();
        }

        public bool Send(string data)
        {
            if (!IsConnected)
            {
                _clientLogger.MessageLog("[OUTGOING] Message was not sent, client is not connected.");
                return false; // if we are not connected, we will just return false
            }
            
            _client.Send(data);
            return true;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}