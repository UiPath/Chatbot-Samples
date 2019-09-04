using Microsoft.Bot.Schema;
using System.Collections.Generic;
using UiPath.ChatbotSamples.BotFramework.Common;
using UiPath.ChatbotSamples.BotFramework.Common.Models;
using UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog;
using UiPath.ChatbotSamples.BotFramework.Resources;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs
{
    public sealed class MyIntents: IBotIntents
    {
        public MyIntents()
        {
            var noneIntent = new Intent("None", null, IntentType.None);
            Intents = IntentsToDictionary(new Intent[] {
                new Intent("Greeting", nameof(GreetingDialog), IntentType.Normal),
                new Intent("DelayedShipment", nameof(DelayedShipmentDialog), IntentType.Normal),
                new Intent("ReplaceItem", nameof(ReplaceItemDialog), IntentType.Normal),
                new TerminationIntent("Cancel", Utils.CreateMessageActivityArray(Resource.Cancel_Success), Utils.CreateMessageActivityArray(Resource.Cancel_Fail)),
                new TerminationIntent("Bye", Utils.CreateMessageActivityArray(Resource.Bye_With_Active_Dialog, Resource.Talk_To_Agent), Utils.CreateMessageActivityArray(Resource.Thank_You, Resource.Bye)),
                new DetourIntent("Help", Utils.CreateMessageActivityArray(Resource.Help, Resource.Talk_To_Agent)),
                noneIntent,
            });
            DefaultIntent = noneIntent;
        }

        public IReadOnlyDictionary<string, Intent> Intents { get; }

        public double IntentTriggerThreshold { get; } = 0.5;

        public Intent DefaultIntent { get; }

        public IActivity FallbackActivity
        {
            get
            {
                return Utils.CreateMessageActivity(Resource.Cannot_Understand);
            }
        }

        public IActivity WelcomeActivity
        {
            get
            {
                return Utils.CreateMessageActivity(Resource.Greeting);
            }
        }

        private Dictionary<string, Intent> IntentsToDictionary(Intent[] intents)
        {
            var dictionary = new Dictionary<string, Intent>();
            foreach (var intent in intents)
            {
                dictionary.Add(intent.Name, intent);
            }
            return dictionary;
        }
    }
}
