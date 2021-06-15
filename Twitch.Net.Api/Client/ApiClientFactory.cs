using System.Net.Http;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.Api.Client
{
    public static class ApiClientFactory
    {
        public static IApiClient CreateClient(
            IApiCredentialConfiguration credentials,
            IHttpClientFactory httpClientFactory,
            ITokenResolver tokenResolver
            )
            => new ApiClient(credentials, httpClientFactory, tokenResolver);
    }
}