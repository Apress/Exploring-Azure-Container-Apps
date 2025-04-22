using BenefitsManager.Common.Models;
using Dapr.Client;

namespace BenefitsManager.Frontend.WebPortal.Ui.Services
{
    public class ClaimsService
    {
        private readonly HttpClient _httpClient;
        private readonly DaprClient _daprClient;

        public ClaimsService(HttpClient httpClient, DaprClient daprClient)
        {
            _httpClient = httpClient;
            _daprClient = daprClient;
        }

        public async Task<List<ClaimModel>> GetClaimsAsync()
        {
            return await _daprClient.InvokeMethodAsync<List<ClaimModel>>(
                HttpMethod.Get,
                "BenefitsManager-Backend-Bff-Api",
                "api/claims/?userId=user3@mail.com"
            );
        }

        public async Task<ClaimModel> GetClaimByIdAsync(Guid id)
        {
            return await _daprClient.InvokeMethodAsync<ClaimModel>(
                HttpMethod.Get,
                "BenefitsManager-Backend-Bff-Api",
                $"api/claims/{id}"
            );
        }

        public async Task<ClaimModel> CreateClaimAsync(ClaimAddModel claim)
        {
            return await _daprClient.InvokeMethodAsync<ClaimAddModel, ClaimModel>(
                HttpMethod.Post,
                "BenefitsManager-Backend-Bff-Api",
                "api/claims",
                claim);
        }

        public async Task UpdateClaimAsync(ClaimModel claim)
        {
            await _daprClient.InvokeMethodAsync(
                HttpMethod.Put,
                "BenefitsManager-Backend-Bff-Api",
                $"api/claims/{claim}",
                claim
            );
        }

        public async Task DeleteClaimAsync(Guid id)
        {
            await _daprClient.InvokeMethodAsync(
                HttpMethod.Delete,
                "BenefitsManager-Backend-Bff-Api",
                $"api/claims/{id}"
            );
        }
    }
}
