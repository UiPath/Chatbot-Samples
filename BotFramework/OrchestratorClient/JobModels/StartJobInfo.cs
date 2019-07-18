namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels
{
    public class StartJobInfo
    {
        public string ReleaseKey { get; set; }

        public string Strategy { get; set; }

        public int JobsCount { get; set; }

        public long[] RobotIds { get; set; }

        public string InputArguments { get; set; }
    }
}
