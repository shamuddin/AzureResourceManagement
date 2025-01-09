using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using AzureManagementAPI.Models;
using AzureManagementAPI.Service.Helper;
using AzureManagementAPI.Service.Interface;
using Microsoft.Extensions.Options;

namespace AzureManagementAPI.Service
{
    public class AzureResourceGroup : IAzureResourceGroup
    {
        private readonly IGetAccessToken _getAccessToken;
        private readonly AzureResourceConfig _azureResourceConfig;
        private readonly AzureAdConfig _azureAdConfig;

        public AzureResourceGroup(IGetAccessToken getAccessToken, IOptions<AzureResourceConfig> azureResourceConfig, IOptions<AzureAdConfig> azureAdConfig)
        {
            _getAccessToken = getAccessToken;
            _azureResourceConfig = azureResourceConfig.Value;
            _azureAdConfig = azureAdConfig.Value;
        }

        /// <summary>
        /// Creates a new resource group in Azure asynchronously.
        /// </summary>
        /// <param name="resourceGroupName">The name of the resource group to be created.</param>
        /// <param name="region">The Azure region where the resource group will be created.</param>
        /// <returns>A task representing the asynchronous operation of creating the resource group.</returns>
        public async Task CreateResourceGroupAsync(string resourceGroupName, string region)
        {
            try
            {
                // Step 1: Get an access token
                var accessToken = await _getAccessToken.GetAccessTokenAsync();

                // Step 2: Wrap the access token in a TokenCredential
                TokenCredential tokenCredential = new GetTokenCredential(accessToken);

                // Step 3: Initialize ArmClient
                ArmClient armClient = new ArmClient(tokenCredential);

                // Step 4: Define the Resource Group data
                var resourceGroupData = new ResourceGroupData(new AzureLocation(region))
                {
                    Tags = { { "Environment", "Dev" } }
                };

                // Step 5: Get the subscription
                SubscriptionResource subscription = await armClient.GetSubscriptionResource(
                    new ResourceIdentifier($"/subscriptions/{_azureResourceConfig.SubscriptionId}")).GetAsync();

                // Step 6: Create or update the resource group
                ResourceGroupCollection resourceGroupCollection = subscription.GetResourceGroups();
                ArmOperation<ResourceGroupResource> operation = await resourceGroupCollection.CreateOrUpdateAsync(
                    WaitUntil.Completed, resourceGroupName, resourceGroupData);

                ResourceGroupResource resourceGroup = operation.Value;

                // Log success
                Console.WriteLine($"Resource group '{resourceGroupName}' created successfully in region '{region}'.");
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error occurred while creating resource group '{resourceGroupName}': {ex.Message}");

                // Optionally handle specific exceptions or log events using _eventService
            }
        }
    }
}
