using System.Threading.Tasks;

namespace Twitch.Net.Utils.Credential
{
    public interface ITokenResolver
    {
        Task<string> GetToken();
        Task<string> GetTokenType();
    }
}