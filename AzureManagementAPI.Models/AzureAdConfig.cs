namespace AzureManagementAPI.Models
{
    public class AzureAdConfig
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ObjectId { get; set; }
    }
}
