using System.Collections.Generic;
using System.Net.Http;
using RateLimiter;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.Api.Apis.Kraken
{
    /**
     * This API is deprecated, so you should be using Helix.
     * Only the none existing API's of Kraken that does not exist in Helix are/will be implemented
     */
    public class ApiKraken : AbstractApiBase, IApiKraken
    {
        public ApiKraken(
            ITokenResolver tokenResolver, 
            IApiCredentialConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            TimeLimiter rateLimiter)
            : base(httpClientFactory, rateLimiter)
        {
            TokenResolver = tokenResolver;
            Credentials = configuration;
        }

        public override IApiCredentialConfiguration Credentials { get; }
        public override string BaseUrl { get; } = "https://api.twitch.tv/kraken";
        public override string ClientIdHeaderKey { get; } = "Client-ID";
        public override IReadOnlyDictionary<string, string> ExtraHeaders { get; } =
            new Dictionary<string, string>
            {
                {"Accept", "application/vnd.twitchtv.v5+json"} // required if you got an older registration
            };

        protected override ITokenResolver TokenResolver { get; }
    }
}