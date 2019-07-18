namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels
{
    public class StartJobResponse
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public string OutputArguments { get; set; } = null;
    }
}
