using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Actions;
using UiPath.ChatbotSamples.BotFramework.Actions.Models;
using UiPath.ChatbotSamples.BotFramework.Common.Models;
using UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog.Base;
using UiPath.ChatbotSamples.BotFramework.Resources;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog
{
    public class ReplaceItemDialog : StatefulDialogBase
    {
        private readonly IMyRpaClient _myRpaClient;
        private readonly IStatePropertyAccessor<EntityState> _entityStateAccessor;
        private const string c_customerId = "CustomerId";

        public ReplaceItemDialog(IMyRpaClient myRPAClient, IStatePropertyAccessor<EntityState> entityStateAccessor) : base(nameof(ReplaceItemDialog), entityStateAccessor)
        {
            var waterfallSteps = new WaterfallStep[]
            {
                GetCustomerIdStepAsync,
                ListItemsStepAsync,
                ProcessItemSelectionStepAsync,
                AskReplacementStepAsync,
                PlaceOrderStepAsync,
            };
            AddDialog(new WaterfallDialog(GetWaterFallDialogId(nameof(ReplaceItemDialog)), waterfallSteps));
            AddDialog(new TextPrompt(nameof(GetCustomerIdStepAsync)));
            AddDialog(new ChoicePrompt(nameof(ListItemsStepAsync)));
            AddDialog(new TextPrompt(nameof(ProcessItemSelectionStepAsync)));
            AddDialog(new ChoicePrompt(nameof(AskReplacementStepAsync)));
            AddDialog(new TextPrompt(nameof(PlaceOrderStepAsync)));

            _myRpaClient = myRPAClient;
            _entityStateAccessor = entityStateAccessor;
        }

        private async Task<DialogTurnResult> GetCustomerIdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(GetCustomerProfileDialog));
        }

        private async Task<DialogTurnResult> ListItemsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(Resource.Working_On);
            var customerId = await TryGetEntityValueAsync(stepContext, c_customerId);
            stepContext.Values[c_customerId] = customerId;

            var result = await _myRpaClient.GetItemsAsync(new GetItemsInput() { CustomerId = customerId });

            if (result?.Items == null || result.Items.Length == 0)
            {
                return await stepContext.BeginDialogAsync(nameof(CannotFindOrderDialog));
            }
            else
            {
                stepContext.Values["Items"] = result;
                var delivered = result.Items.Where(i => i.DeliveryDate < DateTime.Today).Select(i => i.ItemName).ToList();
                delivered.Add(Resource.None_Of_Above);
                return await stepContext.PromptAsync(nameof(ListItemsStepAsync), Utils.CreateMessagePrompt(Resource.Which_Delivered, delivered));
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

                return await stepContext.PromptAsync(nameof(ProcessItemSelectionStepAsync), Utils.CreateMessagePrompt(Resource.Describe_Damage));
            }
        }
    
        private async Task<DialogTurnResult> AskReplacementStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var damage = stepContext.Context.Activity.AsMessageActivity().Text;
            stepContext.Values["Damage"] = damage;
            return await stepContext.PromptAsync(nameof(AskReplacementStepAsync), Utils.CreateMessagePrompt(Resource.Return_Or_Replace, new List<string>(){ Resource.Replace, Resource.Return }));
        }

        private async Task<DialogTurnResult> PlaceOrderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = stepContext.Context.Activity.AsMessageActivity().Text;


            if (message.OrdinalEquals(Resource.Return) || message.OrdinalEquals(Resource.Replace))
            {
                await stepContext.Context.SendActivityAsync(Resource.Working_On);
                var item = (Item)stepContext.Values["SelectedItem"];

                var cancelInput = new CancelOrderInput()
                {
                    OrderId = item.OrderId,
                    CancelReason = (string)stepContext.Values["Damage"],
                };
                var cancelOrderOutput = await _myRpaClient.CancelOrderAsync(cancelInput);
                var orderCancelMessage = string.Format(Resource.Order_Cancelled, item.OrderId, cancelOrderOutput.ReturnLabelLocation);
                await stepContext.Context.SendActivityAsync(orderCancelMessage);

                if (message.OrdinalEquals(Resource.Replace))
                {
                    var salesOrderInput = new CreateSalesOrderInput()
                    {
                        CustomerId = (string)stepContext.Values[c_customerId],
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                    };
                    var createSaledOrderOuput = await _myRpaClient.CreateSalesOrderAsync(salesOrderInput);
                    var orderCreatedMessage = string.Format(Resource.Order_Created, createSaledOrderOuput.OrderId, createSaledOrderOuput.DeliveryDate.ToShortDateString());
                    await stepContext.Context.SendActivityAsync(orderCreatedMessage);                   
                }
                await stepContext.Context.SendActivityAsync(Resource.Anything_Else);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(Resource.Cannot_Understand);
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
