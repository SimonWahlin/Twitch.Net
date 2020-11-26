using Twitch.Net.Api.Apis.Helix;
using Twitch.Net.Api.Apis.Kraken;

namespace Twitch.Net.Api.Client
{
    public interface IApiClient
    {
        IApiKraken ApiKraken { get; }
        IApiHelix Helix { get; }
    }
}