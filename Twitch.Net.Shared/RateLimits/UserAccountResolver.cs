using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Twitch.Net.Shared.Configurations;

namespace Twitch.Net.Shared.RateLimits
{
    /**
     * This is a auto resolver based on kraken - WHICH might and can be removed in the future
     * If the auto resolver for the "user account" stops working you'll have to manually set the information.
     * The reason why this is outside of "API" library too is because of the use-cases it can be used for
     */
    public class UserAccountResolver
    {
        private readonly string BaseUrl = "https://api.twitch.tv/kraken/";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserAccountResolverCredentialConfiguration _configuration;

        public UserAccountResolver(
            IHttpClientFactory httpClientFactory, 
            IUserAccountResolverCredentialConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<UserAccountStatus> TryResolveUserAccountStatusOrDefaultAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_configuration.Username) || 
                    string.IsNullOrEmpty(_configuration.ClientId) ||
                    string.IsNullOrEmpty(_configuration.OAuth))
                    return new UserAccountStatus();
                
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Authorization", 
                    $"OAuth {_configuration.OAuth.Replace("oauth:", "", StringComparison.OrdinalIgnoreCase)}");

                // resolve the actual user Id
                var currentResponse = await client.GetAsync($"{BaseUrl}user?api_version=5&client_id={_configuration.ClientId}");
                if (currentResponse.StatusCode != HttpStatusCode.OK)
                    return new UserAccountStatus();
                var current = await currentResponse.Content.ReadFromJsonAsync<CurrentUser>();

                if (string.IsNullOrEmpty(current?.UserId))
                    return new UserAccountStatus();
                
                // resolve the user account info (undocumented endpoint)
                var userAccountResponse = await client.GetAsync(
                    $"{BaseUrl}users/{current.UserId}/chat?api_version=5&client_id={_configuration.ClientId}");
                if (userAccountResponse.StatusCode != HttpStatusCode.OK)
                    return new UserAccountStatus();
                return await userAccountResponse.Content.ReadFromJsonAsync<UserAccountStatus>();
            }
            catch { /* empty block */ }                            
            return new UserAccountStatus();
        }

        private class CurrentUser
        {
            [JsonPropertyName("_id")]
            public string UserId { get; init; }
        }
    }
}