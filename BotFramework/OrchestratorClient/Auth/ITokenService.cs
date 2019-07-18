using System.Threading.Tasks;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth
{
    public interface ITokenService
    {
        Task<BasicAuthResponse> BasicAuthenticateAsync();

        Task<CloudAuthResponse> CloudAuthenticateAsync();
    }
}
