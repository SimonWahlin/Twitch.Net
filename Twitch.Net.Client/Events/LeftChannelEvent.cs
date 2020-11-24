namespace Twitch.Net.Client.Events
{
    public class LeftChannelEvent
    {
        public string BotUsername { get; init; }
        public string Channel { get; init; }
    }
}