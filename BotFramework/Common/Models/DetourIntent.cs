using Microsoft.Bot.Schema;

namespace UiPath.ChatbotSamples.BotFramework.Common.Models
{
    public class DetourIntent : Intent
    {
        public IActivity[] DetourActivities { get; private set; }

        public DetourIntent(string name, IActivity[] detourActivities)
            : base(name, string.Empty, IntentType.Detour)
        {
            DetourActivities = detourActivities;
        }
    }
}
