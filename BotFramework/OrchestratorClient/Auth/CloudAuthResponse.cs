namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth
{
    public class CloudAuthResponse
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string id_token { get; set; }

        public string scope { get; set;}

        public string expires_in { get; set;}

        public string token_type { get; set; }
    }
}
