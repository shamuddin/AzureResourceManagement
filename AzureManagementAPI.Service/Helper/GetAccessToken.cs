using AzureManagementAPI.Models;
using AzureManagementAPI.Service.Interface;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace AzureManagementAPI.Service.Helper
{
    public class GetAccessToken : IGetAccessToken
    {
        private readonly AzureResourceConfig _azureResourceConfig;
        private readonly IConfidentialClientApplication _app;

        public GetAccessToken(IOptions<AzureAdConfig> azureAdOptions, IOptions<AzureResourceConfig> azureResourceConfig)
        {
            var adOptions = azureAdOptions.Value;
            _azureResourceConfig = azureResourceConfig.Value;
            _app = ConfidentialClientApplicationBuilder.Create(adOptions.ClientId)
                .WithClientSecret(adOptions.ClientSecret)
                .WithAuthority(new Uri($"{adOptions.Instance}{adOptions.TenantId}"))
                .Build();
        }
        public async Task<string> GetAccessTokenAsync()
        {

            var result = await _app.AcquireTokenForClient(new[] { _azureResourceConfig.scope })
             .ExecuteAsync();
            return result.AccessToken;
        }
    }
}
