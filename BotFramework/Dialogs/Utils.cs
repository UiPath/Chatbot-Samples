using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;
using UiPath.ChatbotSamples.BotFramework.Resources;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs
{
    internal static class Utils
    {
        internal static PromptOptions CreateMessagePrompt(string message)
        {
            return new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = message,
                },
            };
        }

        internal static PromptOptions CreateMessagePrompt(string message, List<string> choiceStrings)
        {
            var choices = new List<Choice>();
            choices.AddRange(choiceStrings.Select(s => new Choice(s)));

            return new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = message,
                },
                Choices = choices,
            };
        }

        internal static IMessageActivity CreateMessageActivity(string message)
        {
            var activity = Activity.CreateMessageActivity();
            activity.Text = message;
            return activity;
        }

        internal static IActivity[] CreateMessageActivityArray(params string[] messages)
        {
            var activities = new IActivity[messages.Length];
            for (var i = 0; i < messages.Length; i++)
            {
                activities[i] = CreateMessageActivity(messages[i]);
            }
            return activities;
        }

        internal static List<string> GetYesNoOptions()
        {
            return new List<string>() { Resource.Yes, Resource.No };
        }
    }
}
