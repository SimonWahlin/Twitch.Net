using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using Twitch.Net.Shared.Configurations;

namespace Twitch.Net.Api.Apis
{
    public interface IApiBase
    {
        string BaseUrl { get; }
        string ClientIdHeaderKey { get; }
        IReadOnlyDictionary<string, string> ExtraHeaders { get; }
        IApiCredentialConfiguration Credentials { get; }
        
        /**
         * Pass token if we wanna use a specific access token towards the API endpoint
         */
        Task<Option<T>> GetAsync<T>(
            string segment,
            IReadOnlyList<IReadOnlyDictionary<string, string>> parameters = null, 
            string token = null);
    }
}