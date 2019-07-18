using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using UiPath.ChatbotSamples.BotFramework.Actions;
using UiPath.ChatbotSamples.BotFramework.Common;
using UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs
{
    public class SapBot : MultiTurnBot
    {
        public SapBot(BotServices services, UserState userState, ConversationState conversationState, ISapRpaClient rpaClient, ILoggerFactory loggerFactory)
            :base(services, userState, conversationState, new SapBotSettings(), new SapIntents(), loggerFactory)
        {
            // Must make sure all the dialogs add to dialog set here.
            _dialogSet.AddDialogToSet(new GreetingDialog());
            _dialogSet.AddDialogToSet(new GetCustomerProfileDialog(_entityStateAccessor));
            _dialogSet.AddDialogToSet(new DelayedShipmentDialog(rpaClient, _entityStateAccessor));
            _dialogSet.AddDialogToSet(new ReplaceItemDialog(rpaClient, _entityStateAccessor));
            _dialogSet.AddDialogToSet(new CannotFindOrderDialog());
        }
    }
}
