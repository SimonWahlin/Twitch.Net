using Twitch.Net.Api.Apis.Helix;
using Twitch.Net.Api.Apis.V5;

namespace Twitch.Net.Api.Client
{
    public interface IApiClient
    {
        IApiV5 ApiV5 { get; }
        IApiHelix Helix { get; }
        IApiConfiguration Configuration { get; }
    }
}