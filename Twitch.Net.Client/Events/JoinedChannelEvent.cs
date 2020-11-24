using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Events
{
    public class JoinedChannelEvent
    {
        public string BotUsername { get; init; }
        public ChatChannel Channel { get; init; }
    }
}