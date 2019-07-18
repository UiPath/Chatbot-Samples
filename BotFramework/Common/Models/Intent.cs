using System;

namespace UiPath.ChatbotSamples.BotFramework.Common.Models
{
    public class Intent
    {
        public string Name { get; private set; }

        public string DialogName { get; private set; }

        public IntentType Type { get; private set; }

        public Intent(string name, string dialogName, IntentType type)
        {
            Name = name;
            DialogName = dialogName;
            Type = type;
        }
    }
}
