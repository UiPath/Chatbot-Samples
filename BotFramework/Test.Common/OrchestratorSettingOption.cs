using Microsoft.Extensions.Options;
using System;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient;

namespace UiPath.ChatbotSamples.BotFramework.Test.Common
{
    public class OrchestratorSettingOption : IOptionsMonitor<OrchestratorSettings>
    {
        public OrchestratorSettingOption(OrchestratorSettings setting)
        {
            CurrentValue = setting;
        }

        public OrchestratorSettings CurrentValue { get; }

        public OrchestratorSettings Get(string name)
        {
            throw new NotImplementedException();
        }

        public IDisposable OnChange(Action<OrchestratorSettings, string> listener)
        {
            throw new NotImplementedException();
        }
    }
}
