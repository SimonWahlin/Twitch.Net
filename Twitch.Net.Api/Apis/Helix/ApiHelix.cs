using System.Collections.Generic;
using System.Net.Http;
using RateLimiter;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.Api.Apis.Helix
{
    public class ApiHelix : AbstractApiBase, IApiHelix
    {
        public ApiHelix(
            ITokenResolver tokenResolver, 
            IApiCredentialConfiguration credentials,
            IHttpClientFactory httpClientFactory,
            TimeLimiter rateLimiter)
            : base(httpClientFactory, rateLimiter)
        {
            TokenResolver = tokenResolver;
            Credentials = credentials;

            Users = new Users(this);
        }

        public override string BaseUrl { get; } = "https://api.twitch.tv/helix";
        public override string ClientIdHeaderKey { get; } =  "client-id";
        public override IReadOnlyDictionary<string, string> ExtraHeaders { get; } = new Dictionary<string, string>();
        public override IApiCredentialConfiguration Credentials { get; }
        protected override ITokenResolver TokenResolver { get; }
        
        // API:s
        public Users Users { get; }
    }
}