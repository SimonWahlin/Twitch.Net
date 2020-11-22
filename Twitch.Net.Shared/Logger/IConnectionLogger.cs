namespace Twitch.Net.Shared.Logger
{
    public interface IConnectionLogger
    {
        void Log(string log);
        void MessageLog(string message);
    }
}