namespace Twitch.Net.Shared.Logger
{
    public interface IConnectionLoggerConfiguration
    {
        bool OutputLog { get; set; }
        bool OutputMessageLog { get; set; }
    }
}