using AzureManagementAPI.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AzureManagementController : ControllerBase
    {
        private readonly IAzureSQLServer _azureSQLServer;
        private readonly IAzureResourceGroup _azureResourceGroup;
        private readonly IAzureStorageAccount _azureStorageAccount;

        public AzureManagementController(IAzureSQLServer azureSQLServer, IAzureResourceGroup azureResourceGroup, IAzureStorageAccount azureStorageAccount)
        {
            _azureSQLServer = azureSQLServer;
            _azureResourceGroup = azureResourceGroup;
            _azureStorageAccount = azureStorageAccount;
        }

        [HttpPost("AddResourceGroup")]
        public async Task<IActionResult> AddResourceGroup(string resourceGroupName, string region)
        {
            try
            {
                await _azureResourceGroup.CreateResourceGroupAsync(resourceGroupName, region);
                return Ok($"Resource group '{resourceGroupName}' creation initiated successfully in region '{region}'.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the resource group: {ex.Message}");
            }
        }

        [HttpPost("CreateSqlServer")]
        public async Task<IActionResult> CreateSqlServer(string serverName, string region, string adminUserName, string adminPassword)
        {
            await _azureSQLServer.CreateSqlServerAsync(serverName, region, adminUserName, adminPassword);
            return Ok("SQL Server Successfully Created.");
        }

        [HttpPost("AddStorageAccount")]
        public async Task<IActionResult> AddStorageAccount(string storageAccountName, string resourceGroupName, string region)
        {
            string connectionString = await _azureStorageAccount.CreateStorageAccountAsync(storageAccountName, resourceGroupName, region);
            return Ok(new
            {
                Message = $"Storage account '{storageAccountName}' created successfully in region '{region}'.",
                ConnectionString = connectionString
            });
        }
    }
}
