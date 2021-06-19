using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Optional.Unsafe;
using Twitch.Net.EventSub;
using Twitch.Net.Shared.Extensions;

namespace Twitch.Net.Sample.EventSubServer.Controllers
{
    [ApiController]
    [Route("eventsub")]
    public class EventSubListenerController : Controller
    {
        private readonly IEventSubService _eventSubService;
        private readonly EventSubBuilder _subBuilder;

        public EventSubListenerController(
            IEventSubService eventSubService,
            EventSubBuilder subBuilder
            )
        {
            _eventSubService = eventSubService;
            _subBuilder = subBuilder;
        }

        /**
         * Example of how to handle the callback to verify the EventSub, just pass the request and let
         * the EventSubService handle it all for you!
         */
        [HttpPost("callback")]
        public async Task<IActionResult> HandleEventCallback() =>
            await _eventSubService.Handle(Request);

        [HttpGet("registered")]
        public async Task<IActionResult> GetRegistered()
        {
            var result = await _eventSubService.Subscriptions();

            if (result.HasValue)
                return Ok(result.ValueOrDefault());
            
            return BadRequest();
        }
        
        [HttpGet("clearfailed")]
        public async Task<IActionResult> RemoveFailedOnes()
        {
            var result = await _eventSubService.Subscriptions();

            if (result.HasValue)
            {
                await result.ValueOrDefault().Data
                    .Where(x => x.Status == "webhook_callback_verification_failed")
                    .AsParallel()
                    .ForEachAsync(sub => _eventSubService.Unsubscribe(sub.Id));
                    
                return Ok();
            }
            
            return BadRequest();
        }
        
        [HttpGet("unregisterall")]
        public async Task<IActionResult> RemoveAll()
        {
            var result = await _eventSubService.Subscriptions();

            if (result.HasValue)
            {
                await result.ValueOrDefault().Data
                    .AsParallel()
                    .ForEachAsync(sub => _eventSubService.Unsubscribe(sub.Id));
                    
                return Ok();
            }
            
            return BadRequest();
        }
        
        [HttpGet("testregisterevent")]
        public async Task<IActionResult> TestRegisterSub()
        {
            var model = _subBuilder.Build(EventSubTypes.ChannelFollow, "137132000");

            if (model.HasValue)
            {
                // no token, since we try to use the clientid & clientsecret
                var result = await _eventSubService.Subscribe(model.ValueOrFailure());

                if (result.AlreadyRegistered)
                    return Ok("The specified EventSub is already registered and status enabled");
                
                return result.Output.Match<IActionResult>(
                    Ok,
                    BadRequest
                    );
            }
            
            return BadRequest();
        }
    }
}