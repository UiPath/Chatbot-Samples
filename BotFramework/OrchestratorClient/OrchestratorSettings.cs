using System;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient
{
    public class OrchestratorSettings
    {
        public string BaseUrl { get; set; }

        public string AuthMode { get; set; }

        public string TenancyName { get; set; }

        public string UsernameOrEmailAddress { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }

        public string ServiceInstanceLogicalName { get; set; }

        public string AccountLogicalName { get; set; }

        public int StatusCheckMaxRetry { get; set; } = 60;

        public int StatusCheckInterval { get; set; } = 500;

        public string Strategy { get; set; } = "Specific";

        public int JobsCount { get; set; } = 0;

        public long[] RobotIds { get; set; }

        public ProcessKey[] ProcessKeys { get; set; }

        public bool Validate()
        {
            if (!AuthMode.OrdinalEquals("Basic") && !AuthMode.OrdinalEquals("Cloud"))
            {
                return false;
            }

            if (AuthMode.OrdinalEquals("Basic"))
            {
                if (string.IsNullOrEmpty(TenancyName) || string.IsNullOrEmpty(UsernameOrEmailAddress) || string.IsNullOrEmpty(Password))
                {
                    return false;
                }
            }

            if (AuthMode.OrdinalEquals("Cloud"))
            {
                if (string.IsNullOrEmpty(RefreshToken) || string.IsNullOrEmpty(ServiceInstanceLogicalName) || string.IsNullOrEmpty(AccountLogicalName))
                {
                    return false;
                }
            }

            bool isUrl = Uri.IsWellFormedUriString(BaseUrl, UriKind.Absolute);
            if (!isUrl)
            {
                return false;
            }

            if (!Strategy.OrdinalEquals("Specific") && !Strategy.OrdinalEquals("JobsCount"))
            {
                return false;
            }

            if (Strategy.OrdinalEquals("Specific") && (RobotIds == null || RobotIds.Length == 0))
            {
                return false;
            }

            if (Strategy.OrdinalEquals("JobsCount") && JobsCount <= 0)
            {
                return false;
            }

            if (ProcessKeys == null || ProcessKeys.Length == 0)
            {
                return false;
            }

            foreach (var processKey in ProcessKeys)
            {
                if (!Guid.TryParse(processKey.Key, out Guid newGuid))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
