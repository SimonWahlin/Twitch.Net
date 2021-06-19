namespace Twitch.Net.Lib
{
    public class TwitchLibConfig
    {
        public string CallbackUrl { get; set; } = string.Empty;
        public string SignatureSecret { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string OAuth { get; set; } = string.Empty;
    }
}