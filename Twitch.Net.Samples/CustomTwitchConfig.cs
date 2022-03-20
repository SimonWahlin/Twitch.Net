using Twitch.Net.Shared.Configurations;

namespace Twitch.Net.Samples;

public class CustomTwitchConfig : TwitchCredentialConfiguration
{
    public string BaseChannel { get; set; } // nice to have as "basic" startup
}