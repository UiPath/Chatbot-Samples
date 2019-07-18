using Microsoft.Bot.Schema;
using System.Collections.Generic;
using UiPath.ChatbotSamples.BotFramework.Common.Models;

namespace UiPath.ChatbotSamples.BotFramework.Common
{
    public interface IBotIntents
    {
        IReadOnlyDictionary<string, Intent> Intents { get; }

        double IntentTriggerThreshold { get; }

        Intent DefaultIntent { get; } 

        IActivity FallbackActivity { get; }

        IActivity WelcomeActivity { get; }
    }
}
