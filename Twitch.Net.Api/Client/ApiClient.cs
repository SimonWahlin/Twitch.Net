using System;
using System.Net.Http;
using RateLimiter;
using Twitch.Net.Api.Apis.Helix;
using Twitch.Net.Api.Apis.Kraken;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.Api.Client
{
    internal class ApiClient : IApiClient
    {
        public ApiClient(
            IApiCredentialConfiguration credentials, 
            IHttpClientFactory httpClientFactory, 
            ITokenResolver tokenResolver)
        {
            var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(800, TimeSpan.FromSeconds(60));
            ApiKraken = new ApiKraken(tokenResolver, credentials, httpClientFactory, rateLimiter);
            Helix = new ApiHelix(tokenResolver, credentials, httpClientFactory, rateLimiter);
        }

        public IApiKraken ApiKraken { get; }
        public IApiHelix Helix { get; }
    }
}