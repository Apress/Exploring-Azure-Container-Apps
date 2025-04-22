using Dapr.Client;
using BenefitsManager.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Dapr;

namespace BenefitsManager.Processor.Backend.Svc.Controllers
{
    [Route("api/claimsnotifier")]
    [ApiController]
    public class ClaimsNotifierController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly DaprClient _daprClient;

        public ClaimsNotifierController(IConfiguration config, ILogger<ClaimsNotifierController> logger, DaprClient daprClient)
        {
            _config = config;
            _logger = logger;
            _daprClient = daprClient;
        }

        [Topic("dapr-pubsub-servicebus", "claimsavedtopic")]   // Dapr Pub Sub Service Bus
        [Topic("claimspubsub", "claimsavedtopic")]             // Redis
        [HttpPost("claimsaved")]
        public IActionResult ClaimSaved([FromBody] ClaimModel claimModel)
        {
            const string ClaimsMessage = "Started processing message with Claim Id '{0}'";
            var msg = string.Format(ClaimsMessage, claimModel.ClaimId);
            _logger.LogInformation(ClaimsMessage, claimModel.ClaimId);

            return Ok(msg);
        }
    }
}