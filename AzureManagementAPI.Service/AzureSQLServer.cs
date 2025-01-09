using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Sql;
using AzureManagementAPI.Models;
using AzureManagementAPI.Service.Helper;
using AzureManagementAPI.Service.Interface;
using Microsoft.Extensions.Options;

namespace AzureManagementAPI.Service
{
    public class AzureSQLServer : IAzureSQLServer
    {
        private readonly IGetAccessToken _getAccessToken;
        private readonly AzureResourceConfig _azureResourceConfig;
        private readonly AzureAdConfig _azureAdConfig;

        public AzureSQLServer(IGetAccessToken getAccessToken, IOptions<AzureResourceConfig> azureResourceConfig, IOptions<AzureAdConfig> azureAdConfig)
        {
            _getAccessToken = getAccessToken;
            _azureResourceConfig = azureResourceConfig.Value;
            _azureAdConfig = azureAdConfig.Value;
        }

        public async Task CreateSqlServerAsync(string serverName, string region, string adminUsername, string adminPassword)
        {
            try
            {
                // Step 1: Get an access token
                var accessToken = await _getAccessToken.GetAccessTokenAsync();

                // Step 2: Wrap the access token in a TokenCredential
                TokenCredential tokenCredential = new GetTokenCredential(accessToken);

                // Step 3: Initialize ArmClient
                ArmClient armClient = new ArmClient(tokenCredential);

                // Step 4: Get the Resource Group
                ResourceIdentifier resourceGroupResourceId = ResourceGroupResource.CreateResourceIdentifier(
                    _azureResourceConfig.SubscriptionId,
                    _azureResourceConfig.ResourceGroupName);
                ResourceGroupResource resourceGroup = armClient.GetResourceGroupResource(resourceGroupResourceId);

                // Step 5: Define SQL Server properties
                var sqlServerData = new SqlServerData(new AzureLocation(region))
                {
                    AdministratorLogin = adminUsername,
                    AdministratorLoginPassword = adminPassword,
                    Version = "12.0", // SQL Server version
                    Identity = new ManagedServiceIdentity(ManagedServiceIdentityType.SystemAssigned), // Optional: System-assigned identity
                };

                // Step 6: Get SQL Server collection
                SqlServerCollection sqlServerCollection = resourceGroup.GetSqlServers();

                // Step 7: Create or Update the SQL Server
                ArmOperation<SqlServerResource> operation = await sqlServerCollection.CreateOrUpdateAsync(
                    WaitUntil.Completed,
                    serverName,
                    sqlServerData);

                SqlServerResource sqlServer = operation.Value;

                // Log success
                Console.WriteLine($"SQL Server '{serverName}' successfully created in region '{region}'.");
                Console.WriteLine($"Fully Qualified Domain Name: {sqlServer.Data.FullyQualifiedDomainName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
