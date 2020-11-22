namespace Twitch.Net.Api.Client
{
    public static class ApiClientFactory
    {
        public static IApiClient CreateClient()
            => new ApiClient();
    }
}