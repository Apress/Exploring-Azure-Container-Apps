using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using BenefitsManager.Common.Models;

namespace BenefitsManager.Processor.Backend.Svc.Controllers
{
    public class ExternalClaimsProcessorController : ControllerBase
    {
        private readonly ILogger<ExternalClaimsProcessorController> _logger;
        private readonly DaprClient _daprClient;

        private const string OUTPUT_BINDING_NAME = "externalclaimsblobstore";
        private const string OUTPUT_BINDING_OPERATION = "create";

        public ExternalClaimsProcessorController(ILogger<ExternalClaimsProcessorController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessClaimAndStore([FromBody] ClaimModel claimModel)
        {
            try
            {
                _logger.LogInformation("Started processing external task message from storage queue. claim Name: '{0}'", claimModel.Description);

                claimModel.ClaimId = Guid.NewGuid();
                claimModel.CreatedOn = DateTime.UtcNow.Ticks;

                //Dapr SideCar Invocation (save claim to a state store)
                await _daprClient.InvokeMethodAsync(HttpMethod.Post, "benefitsmanager-backend-api", $"api/claims", claimModel);

                _logger.LogInformation("Saved external claim to the state store successfully. Claim description: '{0}', Claim Id: '{1}'", claimModel.Description, claimModel.ClaimId);

                //code to invoke external binding and store queue message content into blob file in Azure storage
                IReadOnlyDictionary<string, string> metaData = new Dictionary<string, string>()
                    {
                        { "blobName", $"{claimModel.ClaimId}.json" },
                    };

                await _daprClient.InvokeBindingAsync(OUTPUT_BINDING_NAME, OUTPUT_BINDING_OPERATION, claimModel, metaData);

                _logger.LogInformation("Invoked output binding '{0}' for external task. Claim description: '{1}', Claim Id: '{2}'", OUTPUT_BINDING_NAME, claimModel.Description, claimModel.ClaimId);


                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
