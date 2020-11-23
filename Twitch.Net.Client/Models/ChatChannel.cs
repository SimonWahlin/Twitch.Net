namespace Twitch.Net.Client.Models
{
    public class ChatChannel
    {
        public string ChannelName { get; init; }
        public ChatChannelConnectionState ConnectionState { get; private set; } = ChatChannelConnectionState.NotDefined;
        public ChannelStates States { get; init; }

        // is update depending on the result of queue and what not
        internal ChatChannel SetConnectionState(ChatChannelConnectionState state)
        {
            ConnectionState = state;
            return this;
        }
    }

    public enum ChatChannelConnectionState
    {
        NotDefined,
        Queued,
        Connected,
        Failure,
        Connecting
    }
}