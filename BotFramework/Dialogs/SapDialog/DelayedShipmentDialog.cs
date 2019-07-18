using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Actions;
using UiPath.ChatbotSamples.BotFramework.Actions.Models;
using UiPath.ChatbotSamples.BotFramework.Common.Models;
using UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog.Base;
using UiPath.ChatbotSamples.BotFramework.Resources;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog
{
    public class DelayedShipmentDialog : StatefulDialogBase
    {
        private readonly ISapRpaClient _sapRpaClient;
        private readonly IStatePropertyAccessor<EntityState> _entityStateAccessor;

        public DelayedShipmentDialog(ISapRpaClient sapRpaClient, IStatePropertyAccessor<EntityState> entityStateAccessor) 
            : base(nameof(DelayedShipmentDialog), entityStateAccessor)
        {
            var waterfallSteps = new WaterfallStep[]
            {
                GetCustomerIdStepAsync,
                ListItemsStepAsync,
                ProcessItemSelectionStepAsync,
                PlaceOrderStepAsync,
            };
            AddDialog(new WaterfallDialog(GetWaterFallDialogId(nameof(DelayedShipmentDialog)), waterfallSteps));
            AddDialog(new TextPrompt(nameof(GetCustomerIdStepAsync)));
            AddDialog(new ChoicePrompt(nameof(ListItemsStepAsync)));
            AddDialog(new ChoicePrompt(nameof(ProcessItemSelectionStepAsync)));
            AddDialog(new TextPrompt(nameof(PlaceOrderStepAsync)));

            _sapRpaClient = sapRpaClient;
            _entityStateAccessor = entityStateAccessor;
        }

        private async Task<DialogTurnResult> GetCustomerIdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(GetCustomerProfileDialog));
        }

        private async Task<DialogTurnResult> ListItemsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(Resource.Working_On);
            var customerId = await TryGetEntityValueAsync(stepContext, "CustomerId");
            stepContext.Values["CustomerId"] = customerId;

            var result = await _sapRpaClient.GetItemsAsync(new GetItemsInput() { CustomerId = customerId });

            if (result?.Items == null || result.Items.Length == 0)
            {
                return await stepContext.BeginDialogAsync(nameof(CannotFindOrderDialog));
            }
            else
            {
                stepContext.Values["Items"] = result;
                var unDelivered = result.Items.Where(i => i.DeliveryDate >= DateTime.Today).Select(i => i.ItemName).ToList();
                unDelivered.Add(Resource.None_Of_Above);               
                return await stepContext.PromptAsync(nameof(ListItemsStepAsync), Utils.CreateMessagePrompt(Resource.Which_Undelivered, unDelivered));
            }          
        }

        private async Task<DialogTurnResult> ProcessItemSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = stepContext.Context.Activity.AsMessageActivity().Text;
            if (message.OrdinalEquals(Resource.None_Of_Above))
            {
                return await stepContext.BeginDialogAsync(nameof(CannotFindOrderDialog));
            }
            else
            {
                var matchedItems = ((GetItemsOutput)stepContext.Values["Items"]).Items.Where(i => i.ItemName.OrdinalEquals(message)).ToList();
                if (matchedItems.Count == 0)
                {
                    return await stepContext.BeginDialogAsync(nameof(CannotFindOrderDialog));
                }

                stepContext.Values["SelectedItem"] = matchedItems[0];

                return await stepContext.PromptAsync(nameof(ProcessItemSelectionStepAsync), Utils.CreateMessagePrompt(string.Format(Resource.Deliver_Status, matchedItems[0].DeliveryStatus), Utils.GetYesNoOptions()));
            }
        }

        private async Task<DialogTurnResult> PlaceOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var message = stepContext.Context.Activity.AsMessageActivity().Text;
            if (message.OrdinalEquals(Resource.Yes))
            {
                await stepContext.Context.SendActivityAsync(Resource.Working_On);
                var item = (Item)stepContext.Values["SelectedItem"];
                var purchaseOrderOutput = await _sapRpaClient.CreatePurchaseOrderAsync(new CreatePurchaseOrderInput() { ItemId = item.ItemId });

                var cancelInput = new CancelOrderInput()
                {
                    OrderId = item.OrderId,
                    CancelReason = "Delayed shipment",
                };
                var cancelOrderOutput = await _sapRpaClient.CancelOrderAsync(cancelInput);
                var orderCancelMessage = string.Format(Resource.Order_Cancelled, item.OrderId, cancelOrderOutput.ReturnLabelLocation);
                await stepContext.Context.SendActivityAsync(orderCancelMessage);

                var salesOrderInput = new CreateSalesOrderInput()
                {
                    CustomerId = (string)stepContext.Values["CustomerId"],
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                };
                var createSaledOrderOuput = await _sapRpaClient.CreateSalesOrderAsync(salesOrderInput);
                var orderCreatedMessage = string.Format(Resource.Order_Created, createSaledOrderOuput.OrderId, createSaledOrderOuput.DeliveryDate.ToShortDateString());
                await stepContext.Context.SendActivityAsync(orderCreatedMessage);
            }
            await stepContext.Context.SendActivityAsync(Resource.Anything_Else);
            return await stepContext.EndDialogAsync();
        }
    }
}
