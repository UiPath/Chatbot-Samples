using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog.Base;
using UiPath.ChatbotSamples.BotFramework.Resources;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog
{
    public class GreetingDialog : DialogBase
    {
        public GreetingDialog()
            : base(nameof(GreetingDialog))
        {
            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                ShowHelpStepAsync,
            };
            AddDialog(new WaterfallDialog(GetWaterFallDialogId(nameof(GreetingDialog)), waterfallSteps));
            AddDialog(new TextPrompt(nameof(ShowHelpStepAsync)));
        }

        private async Task<DialogTurnResult> ShowHelpStepAsync(
                                                WaterfallStepContext stepContext,
                                                CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(Resource.Help);
            return await stepContext.EndDialogAsync();
        }
    }
}
