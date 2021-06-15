using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Twitch.Net.EventSub;
using Twitch.Net.EventSub.Models;

namespace Twitch.Net.Sample.EventSubServer.Controllers
{
    [ApiController]
    [Route("eventsub")]
    public class EventSubListenerController : Controller
    {
        private readonly ILogger<EventSubListenerController> _logger;
        private readonly IEventSubService _eventSubService;

        public EventSubListenerController(
            ILogger<EventSubListenerController> logger,
            IEventSubService eventSubService
            )
        {
            _logger = logger;
            _eventSubService = eventSubService;
        }

        /**
         * Example of how to handle the callback to verify the EventSub, just pass the request and let
         * the EventSubService and it'll handle and verify everything for you!
         */
        [HttpPost("callback")]
        public async Task<IActionResult> HandleEventCallback()
        {
            try
            {
                var result = await _eventSubService.Handle(Request);

               return result ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
    }
}