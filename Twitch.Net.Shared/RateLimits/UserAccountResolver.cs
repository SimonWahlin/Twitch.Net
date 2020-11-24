using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Twitch.Net.Shared.Configurations;
using Twitch.Net.Shared.Credential;

namespace Twitch.Net.Shared.RateLimits
{
    /**
     * This is a auto resolver based on kraken - WHICH might and can be removed in the future
     * If the auto resolver for the "user account" stops working you'll have to manually set the information.
     * The reason why this is outside of "API" library too is because of the use-cases it can be used for
     */
    public class UserAccountResolver
    {
        private const string KrakenBaseUrl = "https://api.twitch.tv/kraken/";
        private const string HelixBaseUrl = "https://api.twitch.tv/helix/";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserAccountResolverCredentialConfiguration _configuration;
        private readonly ClientCredentialTokenResolver _clientTokenResolver;

        public UserAccountResolver(
            IHttpClientFactory httpClientFactory, 
            IUserAccountResolverCredentialConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _clientTokenResolver = new ClientCredentialTokenResolver(_httpClientFactory, configuration);
        }

        public async Task<UserAccountStatus> TryResolveUserAccountStatusOrDefaultAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_configuration.Username) || 
                    string.IsNullOrEmpty(_configuration.ClientId) ||
                    string.IsNullOrEmpty(_configuration.ClientSecret))
                    return new UserAccountStatus();

                var token = await _clientTokenResolver.GetToken();
                var type = await _clientTokenResolver.GetTokenType();
                if (string.IsNullOrEmpty(token))
                    return new UserAccountStatus();
                
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"{type} {token}");
                client.DefaultRequestHeaders.Add("Client-Id", $"{_configuration.ClientId}");

                // resolve the actual user Id
                var currentResponse = await client.GetAsync($"{HelixBaseUrl}users?login={_configuration.Username}");
                if (currentResponse.StatusCode != HttpStatusCode.OK)
                    return new UserAccountStatus();
                var currentResult = await currentResponse.Content.ReadFromJsonAsync<UsersResponse>();

                if (currentResult == null || !currentResult.Users.Any() || string.IsNullOrEmpty(currentResult.Users[0].UserId))
                    return new UserAccountStatus();
                
                // resolve the user account info (undocumented endpoint)
                var userAccountResponse = await client.GetAsync(
                    $"{KrakenBaseUrl}users/{currentResult.Users[0].UserId}/chat?api_version=5&client_id={_configuration.ClientId}");
                if (userAccountResponse.StatusCode != HttpStatusCode.OK)
                    return new UserAccountStatus();
                return await userAccountResponse.Content.ReadFromJsonAsync<UserAccountStatus>();
            }
            catch { /* empty block */ }                            
            return new UserAccountStatus();
        }

        private class UsersResponse
        {
            [JsonPropertyName("data")]
            public CurrentUser[] Users { get; init; }
        }

        private class CurrentUser
        {
            [JsonPropertyName("id")]
            public string UserId { get; init; }
        }
    }
}