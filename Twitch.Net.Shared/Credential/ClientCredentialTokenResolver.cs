using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Twitch.Net.Shared.Configurations;

namespace Twitch.Net.Shared.Credential
{
    /**
     * Recommendation is to only create ONE instance of this to use through the whole application.
     * This is a basic implementation for client credential, if you gonna use the access token of a user
     * it is highly recommended to implement your own solution of "ITokenResolver".
     */
    public class ClientCredentialTokenResolver
    {
        private const string TwitchApiAuthEndpoint = "https://id.twitch.tv/oauth2/token";

        private string _token = string.Empty;
        private string _tokenType = string.Empty;
        private DateTime _tokenExpiration = DateTime.Now;
        private readonly SemaphoreSlim _semaphoreSlim = new(1,1);
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TwitchCredentialConfiguration _configuration;

        public ClientCredentialTokenResolver(
            IHttpClientFactory httpClientFactory,
            TwitchCredentialConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetToken()
        {
            if (IsTokenExpired())
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    var response = await Authenticate();
                    if (response != null)
                    {
                        _tokenExpiration = DateTime.Now.AddSeconds(response.ExpiresIn);
                        _token = response.AccessToken;
                        _tokenType = response.TokenType;
                    }
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
            
            return _token;
        }

        public async Task<string> GetTokenType()
        {
            await GetToken(); // this is to make sure it is up to date
            return _tokenType;
        }

        public async Task<DateTime> GetExpirationDate()
        {
            await GetToken(); // this is to make sure it is up to date
            return _tokenExpiration;
        }

        public bool IsTokenExpired() => _tokenExpiration < DateTime.UtcNow;

        /**
         * This is to get client credentials, it is similar if you need to get another type of token but
         * you may need to pass more stuff & the scopes you are in need of
         */
        private async Task<TwitchClientCredentialAuthResponse> Authenticate()
        {
            if (!IsTokenExpired()) // already updated (semaphore check)
                return null;
            
            using var client = _httpClientFactory.CreateClient();
            var @params = new Dictionary<string, string>
            {
                { "client_id", _configuration.ClientId },
                { "client_secret", _configuration.ClientSecret },
                { "grant_type", "client_credentials" },
                { "scopes", "" }
            };
            var content = new FormUrlEncodedContent(@params);

            var response = await client.PostAsync(TwitchApiAuthEndpoint, content);
            if (response.StatusCode == HttpStatusCode.OK)
                return await response.Content.ReadFromJsonAsync<TwitchClientCredentialAuthResponse>();

            return null; // something went wrong
        }
    }
}