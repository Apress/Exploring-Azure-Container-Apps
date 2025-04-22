using BenefitsManager.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BenefitsManager.Backend.Bff.Api.Controllers
{
    [Route("api/overdueclaims")]
    [ApiController]
    public class OverdueClaimsController : ControllerBase
    {
        private readonly ILogger<OverdueClaimsController> _logger;
        private readonly IClaimsManager _claimsManager;

        public OverdueClaimsController(ILogger<OverdueClaimsController> logger, IClaimsManager tasksManager)
        {
            _logger = logger;
            _claimsManager = tasksManager;
        }

        [HttpGet]
        public async Task<IEnumerable<ClaimModel>> Get()
        {
            return await _claimsManager.GetYesterdaysDueClaims();
        }

        [HttpPost("markoverdue")]
        public async Task<IActionResult> Post([FromBody] List<ClaimModel> overdueClaimsList)
        {
            await _claimsManager.MarkOverdueClaims(overdueClaimsList);

            return Ok();
        }
    }
}
