using System;
using System.Globalization;

namespace Twitch.Net.Utils.Logger
{
    public class ConnectionLogger : IConnectionLogger, IConnectionLoggerConfiguration
    {
        public void Log(string log)
        {
            if (OutputLog && !string.IsNullOrEmpty(log))
                Console.WriteLine($"[{Time()}] {log.TrimEnd()}");
        }

        public void MessageLog(string message)
        {
            if (OutputMessageLog && !string.IsNullOrEmpty(message))
                Console.WriteLine($"[{Time()}] {message.TrimEnd()}");
        }

        private string Time() =>
            DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);

        public bool OutputLog { get; set; }
        public bool OutputMessageLog { get; set; }
    }
}