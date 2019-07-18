using System.Collections.Generic;
using UiPath.ChatbotSamples.BotFramework.Common.Models;

namespace UiPath.ChatbotSamples.BotFramework.Common
{
    public abstract class BotSettings
    {
        public abstract string LuisConfiguration { get; }


        public abstract IReadOnlyDictionary<string, LuisEntity> Entities { get; }
    }
}
