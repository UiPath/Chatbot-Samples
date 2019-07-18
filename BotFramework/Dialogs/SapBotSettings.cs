using System.Collections.Generic;
using UiPath.ChatbotSamples.BotFramework.Common;
using UiPath.ChatbotSamples.BotFramework.Common.Models;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs
{
    public sealed class SapBotSettings: BotSettings
    {
        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public override string LuisConfiguration {
            get {
                return "Sample";
            }
        }
        
        public override IReadOnlyDictionary<string, LuisEntity> Entities { get; }
        = new Dictionary<string, LuisEntity>
        {
            { "CustomerId", new LuisEntity(new List<string>(){ "Id" }, NomalizeCustomerId) },
        };

        private static string NomalizeCustomerId(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.ToUpperInvariant();
            }
            return value;
        }
    }
}
