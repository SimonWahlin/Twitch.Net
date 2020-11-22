using Twitch.Net.Api.Apis.Helix;
using Twitch.Net.Api.Apis.V5;

namespace Twitch.Net.Api.Client
{
    internal class ApiClient : IApiClient
    {
        public ApiClient()
        {
            ApiV5 = new ApiV5();
            Helix = new ApiHelix();
            Configuration = new ApiConfiguration();
        }

        public IApiV5 ApiV5 { get; }
        public IApiHelix Helix { get; }
        public IApiConfiguration Configuration { get; }
    }
}