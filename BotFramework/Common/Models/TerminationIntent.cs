using Microsoft.Bot.Schema;

namespace UiPath.ChatbotSamples.BotFramework.Common.Models
{
    public class TerminationIntent : Intent
    {
        public IActivity[] TerminationSuccessActivities { get; private set; }

        public IActivity[] TerminationFailActivities { get; private set; }

        public TerminationIntent(string name, IActivity[] successActivities, IActivity[] failActivities)
            : base(name, string.Empty, IntentType.Termination)
        {
            TerminationSuccessActivities = successActivities;
            TerminationFailActivities = failActivities;
        }
    }
}
