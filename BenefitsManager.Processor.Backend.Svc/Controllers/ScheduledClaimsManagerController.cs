using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using BenefitsManager.Common.Models;

namespace BenefitsManager.Processor.Backend.Svc.Controllers
{
    [Route("ScheduledClaimsManager")]
    [ApiController]
    public class ScheduledClaimsManagerController : ControllerBase
    {
        private readonly ILogger<ScheduledClaimsManagerController> _logger;
        private readonly DaprClient _daprClient;
        public ScheduledClaimsManagerController(ILogger<ScheduledClaimsManagerController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpPost]
        public async Task CheckOverDueClaimsJob()
        {
            var runAt = DateTime.UtcNow;

            _logger.LogInformation($"ScheduledClaimsManager::Timer Services triggered at: {runAt}");

            var overdueClaimsList = new List<ClaimModel>();

            var claimsList = await _daprClient.InvokeMethodAsync<List<ClaimModel>>(HttpMethod.Get, "benefitsmanager-backend-api", $"api/overdueclaims");

            _logger.LogInformation($"ScheduledClaimsManager::completed query state store for claims, retrieved claims count: {claimsList?.Count()}");

            claimsList?.ForEach(claimModel =>
            {
                var purchaseDate = DateTimeOffset.FromUnixTimeMilliseconds(claimModel.PurchaseDate).UtcDateTime;
                if (runAt.Date > purchaseDate.Date)
                {
                    overdueClaimsList.Add(claimModel);
                }
            });

            if (overdueClaimsList.Count > 0)
            {
                _logger.LogInformation($"ScheduledClaimsManager::marking {overdueClaimsList.Count()} as overdue claims");

                await _daprClient.InvokeMethodAsync(HttpMethod.Post, "benefitsmanager-backend-api", $"api/overdueclaims/markoverdue", overdueClaimsList);
            }
        }
    }
}
