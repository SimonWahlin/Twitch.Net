using System.Threading.Tasks;
using Twitch.Net.Client.Client.Handlers.Events;
using Twitch.Net.Client.Irc;

namespace Twitch.Net.Client.Events.Handlers
{
    public class MessageEventHandler : IHandler
    {
        public async Task<bool> Handle(IIrcClientEventInvoker eventInvoker, IrcMessage message)
        {
            if (message.HostMask.Equals("jtv!jtv@jtv.tmi.twitch.tv"))
                await eventInvoker.InvokeOnBeingHosted(new BeingHostedEvent(message));
            else if (!string.IsNullOrEmpty(message.Message))
                await eventInvoker.InvokeOnMessage(new ChatMessageEvent(message));
            else
                return false;
            
            return true;
        }
    }
}