using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Common.Models;
using UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog.Base;
using UiPath.ChatbotSamples.BotFramework.Resources;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog
{
    public class GetCustomerProfileDialog: StatefulDialogBase
    {
        private readonly IStatePropertyAccessor<EntityState> _entityStateAccessor;

        public GetCustomerProfileDialog(IStatePropertyAccessor<EntityState> entityStateAccessor) : base(nameof(GetCustomerProfileDialog), entityStateAccessor)
        {
            var waterfallSteps = new WaterfallStep[]
            {
                PromptForCustomerIdStepAsync,
                ProcessCustomerIdStepAsync,
            };
            AddDialog(new WaterfallDialog(GetWaterFallDialogId(nameof(GetCustomerProfileDialog)), waterfallSteps));
            AddDialog(new TextPrompt(nameof(PromptForCustomerIdStepAsync)));
            AddDialog(new TextPrompt(nameof(ProcessCustomerIdStepAsync)));

            _entityStateAccessor = entityStateAccessor;
        }
        private async Task<DialogTurnResult> PromptForCustomerIdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdAsync(stepContext);
            if (string.IsNullOrEmpty(customerId))
            {
                return await stepContext.PromptAsync(nameof(PromptForCustomerIdStepAsync), Utils.CreateMessagePrompt(Resource.Ask_Customer_Id));
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }

        private async Task<DialogTurnResult> ProcessCustomerIdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var customerId = await GetCustomerIdAsync(stepContext);
            if (string.IsNullOrEmpty(customerId))
            {
                // loop until get customer id
                return await stepContext.ReplaceDialogAsync(nameof(GetCustomerProfileDialog), cancellationToken);
            }
            return await stepContext.Parent.ContinueDialogAsync();
        }

        private async Task<string> GetCustomerIdAsync(WaterfallStepContext stepContext)
        {
            return await TryGetEntityValueAsync(stepContext, "CustomerId");
        }
    }
}
