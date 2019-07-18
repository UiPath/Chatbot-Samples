using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient
{
    public interface IOrchestratorClient
    {
        Task<StartJobResponse> ExecuteJobAsync(StartJobInfo jobInfo);
    }
}
