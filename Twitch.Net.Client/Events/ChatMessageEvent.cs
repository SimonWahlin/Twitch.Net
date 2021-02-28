using System.Collections.Generic;
using Twitch.Net.Client.Extensions;
using Twitch.Net.Client.Irc;
using Twitch.Net.Client.Models;

namespace Twitch.Net.Client.Events
{
    public class ChatMessageEvent
    {
        private readonly IReadOnlyDictionary<string, string> _tags;
        private readonly string _username;
        public string Message { get; }
        public bool IsMeAction { get; }
        public string Channel { get; }
        
        internal ChatMessageEvent(IrcMessage ircMessage)
        {
            _tags = ircMessage.Tags;
            _username = ircMessage.User;
            
            Message = IsIncomingMeAction(ircMessage.Message)
                ? ircMessage.Message.Trim('\u0001').Substring(7)
                : ircMessage.Message;
            IsMeAction = IsIncomingMeAction(ircMessage.Message);
            Channel = ircMessage.Channel;
        }

        public string DisplayName => 
            _tags.ContainsKey(TwitchIrcMessageTags.DisplayName) && 
            !string.IsNullOrEmpty(_tags[TwitchIrcMessageTags.DisplayName])
                ? _tags[TwitchIrcMessageTags.DisplayName]
                : _username;

        public string Username => _username;

        public bool IsModerator =>
            _tags.TagToBoolean(TwitchIrcMessageTags.Mod);

        public bool IsSubscriber =>
            _tags.ParseBadges().ContainsKey("founder") || 
            _tags.ParseBadges().ContainsKey("subscriber") ||
            _tags.TagToBoolean(TwitchIrcMessageTags.Subscriber);

        public bool IsTurbo =>
            _tags.ParseBadges().ContainsKey("turbo") ||
            _tags.TagToBoolean(TwitchIrcMessageTags.Turbo);

        public UserType Type =>
            _tags.ContainsKey(TwitchIrcMessageTags.UserType)
                ? _tags[TwitchIrcMessageTags.UserType] switch
                {
                    "mod" => UserType.Moderator,
                    "global_mod" => UserType.GlobalModerator,
                    "admin" => UserType.Admin,
                    "staff" => UserType.Staff,
                    _ => UserType.Viewer
                }
                : UserType.Viewer;

        private bool IsIncomingMeAction(string msg)
            => msg.Length > 0 && (byte) msg[0] == 1 && (byte) msg[^1] == 1 &&
               msg.StartsWith("\u0001ACTION ") && msg.EndsWith("\u0001");
    }
}