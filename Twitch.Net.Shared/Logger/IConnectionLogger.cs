namespace Twitch.Net.Utils.Logger
{
    public interface IConnectionLogger
    {
        void Log(string log);
        void MessageLog(string message);
    }
}