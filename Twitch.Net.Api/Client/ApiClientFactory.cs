namespace Twitch.Net.Api.Client
{
    public class ApiClientFactory
    {
        public IApiClient CreateClient()
            => new ApiClient();
    }
}