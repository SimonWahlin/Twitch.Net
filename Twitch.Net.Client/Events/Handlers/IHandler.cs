using System.Threading.Tasks;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Irc;

namespace Twitch.Net.Client.Events.Handlers
{
    public interface IHandler
    {
        Task<bool> Handle(IIrcClientEventInvoker eventInvoker, IrcMessage message);
    }
}