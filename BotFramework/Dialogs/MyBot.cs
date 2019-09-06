using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using UiPath.ChatbotSamples.BotFramework.Actions;
using UiPath.ChatbotSamples.BotFramework.Common;
using UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs
{
    public class MyBot : MultiTurnBot
    {
        public MyBot(BotServices services, UserState userState, ConversationState conversationState, IMyRpaClient rpaClient, ILoggerFactory loggerFactory)
            :base(services, userState, conversationState, new MyBotSettings(), new MyIntents(), loggerFactory)
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
