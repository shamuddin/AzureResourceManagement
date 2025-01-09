namespace AzureManagementAPI.Service.Interface
{
    public interface IGetAccessToken
    {
        Task<string> GetAccessTokenAsync();
    }
}
