using Azure.Core;

namespace AzureManagementAPI.Service.Helper
{
    public class GetTokenCredential : TokenCredential
    {
        private readonly string _accessToken;

        public GetTokenCredential(string accessToken)
        {
            _accessToken = accessToken;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new AccessToken(_accessToken, DateTimeOffset.MaxValue);
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new ValueTask<AccessToken>(new AccessToken(_accessToken, DateTimeOffset.MaxValue));
        }
    }
}
