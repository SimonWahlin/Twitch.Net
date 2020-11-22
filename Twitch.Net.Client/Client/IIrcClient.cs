using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Net.Client.Models;
using Twitch.Net.Shared.Logger;

namespace Twitch.Net.Client.Client
{
    public interface IIrcClient
    {
        IReadOnlyList<ChatChannel> Channels { get; }
        IConnectionLoggerConfiguration ConnectionLoggerConfiguration { get; }
        IIrcClientEventHandler Events { get; }
        
        Task<bool> ConnectAsync();
        bool SendMessage(ChatChannel chatChannel, string message);
        bool SendMessage(string channel, string message);
        bool SendRaw(string message);
        ChatChannel JoinChannel(string channel);
    }
}