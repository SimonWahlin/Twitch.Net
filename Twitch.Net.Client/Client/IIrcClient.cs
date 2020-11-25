using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Client
{
    public interface IIrcClient
    {
        IReadOnlyList<ChatChannel> Channels { get; }
        IIrcClientEventHandler Events { get; }
        bool IsConnected { get; }
        string BotUsername { get; }
        
        Task<bool> ConnectAsync();
        bool SendMessage(ChatChannel chatChannel, string message);
        bool SendMessage(string channel, string message);
        bool SendRaw(string message);
        ChatChannel JoinChannel(string channel);
    }
}