
namespace AzureManagementAPI.Service.Interface
{
    public interface IAzureResourceGroup
    {
        Task CreateResourceGroupAsync(string resourceGroupName, string region);
    }
}
