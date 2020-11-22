namespace Twitch.Net.Utils.Logger
{
    public interface IConnectionLoggerConfiguration
    {
        bool OutputLog { get; set; }
        bool OutputMessageLog { get; set; }
    }
}