
namespace AzureManagementAPI.Service.Interface
{
    public interface IAzureStorageAccount
    {
        Task<string> CreateStorageAccountAsync(string storageAccountName, string resourceGroupName, string region);
    }
}
