namespace AzureManagementAPI.Service.Interface
{
    public interface IAzureSQLServer
    {
        Task CreateSqlServerAsync(string serverName, string region, string adminUsername, string adminPassword);
    }
}
