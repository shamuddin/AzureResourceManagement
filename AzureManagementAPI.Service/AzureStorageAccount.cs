using Azure.Core;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage.Models;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager;
using Azure;
using AzureManagementAPI.Models;
using AzureManagementAPI.Service.Interface;
using Microsoft.Extensions.Options;
using AzureManagementAPI.Service.Helper;

namespace AzureManagementAPI.Service
{
    public class AzureStorageAccount : IAzureStorageAccount
    {
        private readonly IGetAccessToken _getAccessToken;
        private readonly AzureResourceConfig _azureResourceConfig;
        private readonly AzureAdConfig _azureAdConfig;

        public AzureStorageAccount(IGetAccessToken getAccessToken, IOptions<AzureResourceConfig> azureResourceConfig, IOptions<AzureAdConfig> azureAdConfig)
        {
            _getAccessToken = getAccessToken;
            _azureResourceConfig = azureResourceConfig.Value;
            _azureAdConfig = azureAdConfig.Value;
        }

        /// <summary>
        /// Creates a new Azure Storage Account asynchronously in a specified resource group and region and returns the connection string.
        /// </summary>
        /// <param name="storageAccountName">The name of the storage account to be created.</param>
        /// <param name="resourceGroupName">The name of the resource group where the storage account will be created.</param>
        /// <param name="region">The Azure region where the storage account will be created.</param>
        /// <returns>The connection string of the created storage account.</returns>
        public async Task<string> CreateStorageAccountAsync(string storageAccountName, string resourceGroupName, string region)
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
                    _azureResourceConfig.SubscriptionId, resourceGroupName);
                ResourceGroupResource resourceGroup = armClient.GetResourceGroupResource(resourceGroupResourceId);

                // Step 5: Define the Storage Account data
                StorageAccountCreateOrUpdateContent storageAccountData = new StorageAccountCreateOrUpdateContent(
                    new StorageSku(StorageSkuName.StandardRagrs),
                    StorageKind.StorageV2,
                    new AzureLocation(region))
                {
                    AccessTier = StorageAccountAccessTier.Hot,
                    Tags = { { "Environment", "Dev"} },
                    AllowBlobPublicAccess = true,
                    AllowSharedKeyAccess = true,
                    MinimumTlsVersion = StorageMinimumTlsVersion.Tls1_2,
                    AllowCrossTenantReplication = true,
                    DnsEndpointType = StorageDnsEndpointType.Standard,
                    IsDefaultToOAuthAuthentication = false,
                    PublicNetworkAccess = StoragePublicNetworkAccess.Enabled,
                };

                // Step 6: Get the Storage Account collection
                StorageAccountCollection storageAccountCollection = resourceGroup.GetStorageAccounts();

                // Step 7: Create or Update the Storage Account
                ArmOperation<StorageAccountResource> operation = await storageAccountCollection.CreateOrUpdateAsync(
                    WaitUntil.Completed, storageAccountName, storageAccountData);

                StorageAccountResource storageAccount = operation.Value;

                // Step 8: Retrieve the connection string
                string connectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={await GetAccountKeyAsync(armClient, storageAccountName, resourceGroupName)};EndpointSuffix=core.windows.net";

                // Log success
                Console.WriteLine($"Storage account '{storageAccountName}' created successfully in region '{region}'.");

                return connectionString;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error occurred while creating storage account '{storageAccountName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the primary key of a storage account.
        /// </summary>
        /// <param name="armClient">The ArmClient used to manage Azure resources.</param>
        /// <param name="storageAccountName">The name of the storage account.</param>
        /// <param name="resourceGroupName">The name of the resource group containing the storage account.</param>
        /// <returns>The primary key of the storage account.</returns>
        private async Task<string> GetAccountKeyAsync(ArmClient armClient, string storageAccountName, string resourceGroupName)
        {
            // Get the resource group
            var resourceGroupResourceId = ResourceGroupResource.CreateResourceIdentifier(
                _azureResourceConfig.SubscriptionId, resourceGroupName);
            var resourceGroup = armClient.GetResourceGroupResource(resourceGroupResourceId);

            // Get the storage account
            var storageAccount = await resourceGroup.GetStorageAccounts().GetAsync(storageAccountName);

            // Retrieve the storage account keys
            var keys = storageAccount.Value.GetKeysAsync();

            // Get the first key (primary key)
            await foreach (var key in keys)
            {
                return key.Value; // Return the primary key
            }

            throw new InvalidOperationException("No keys were found for the storage account.");
        }
    }
}
