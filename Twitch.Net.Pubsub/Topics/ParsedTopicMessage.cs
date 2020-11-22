namespace Twitch.Net.PubSub.Topics
{
    /**
     * An object holder of a message which was a TOPIC
     */
    internal class ParsedTopicMessage
    {
        public bool Parsed { get; init; }
        public string Topic { get; init; }
        public string JsonData { get; init; }
    }
}