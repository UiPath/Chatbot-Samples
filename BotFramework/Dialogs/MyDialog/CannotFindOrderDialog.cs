using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog.Base;
using UiPath.ChatbotSamples.BotFramework.Resources;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog
{
    public class CannotFindOrderDialog : DialogBase
    {
        public CannotFindOrderDialog()
            : base(nameof(CannotFindOrderDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                NoUndeliveredOrderStepAsync,
                ProcessTalkToAgentStepAsync,
            };
            AddDialog(new WaterfallDialog(GetWaterFallDialogId(nameof(CannotFindOrderDialog)), waterfallSteps));
            AddDialog(new ConfirmPrompt(nameof(NoUndeliveredOrderStepAsync)));
            AddDialog(new TextPrompt(nameof(ProcessTalkToAgentStepAsync)));
        }

        private async Task<DialogTurnResult> NoUndeliveredOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(NoUndeliveredOrderStepAsync), Utils.CreateMessagePrompt(Resource.No_Undelivered_Order, Utils.GetYesNoOptions()));
        }

        private async Task<DialogTurnResult> ProcessTalkToAgentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = stepContext.Context.Activity.AsMessageActivity().Text;
            if (message.OrdinalEquals(Resource.Yes))
            {
                await stepContext.Context.SendActivityAsync(Resource.Talk_To_Agent);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(Resource.Anything_Else);
            }
            await stepContext.EndDialogAsync();
            return await stepContext.Parent.CancelAllDialogsAsync();
        }
    }
}
