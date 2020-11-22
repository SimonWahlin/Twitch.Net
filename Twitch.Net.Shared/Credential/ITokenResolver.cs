using System.Threading.Tasks;

namespace Twitch.Net.Shared.Credential
{
    public interface ITokenResolver
    {
        Task<string> GetToken();
        Task<string> GetTokenType();
    }
}