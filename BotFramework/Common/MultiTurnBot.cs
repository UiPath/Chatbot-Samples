using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Common.Models;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.Common
{
    public class MultiTurnBot : IBot
    {
        protected readonly UserState _userState;
        protected readonly ConversationState _conversationState;
        protected readonly LuisRecognizer _luisRecognizer;
        protected readonly BotDialogSet _dialogSet;
        protected readonly BotSettings _botSettings;
        protected readonly IBotIntents _botIntents;
        protected readonly ILogger<MultiTurnBot> _logger;

        protected readonly IStatePropertyAccessor<EntityState> _entityStateAccessor;

        public MultiTurnBot(BotServices services, UserState userState, ConversationState conversationState, BotSettings botSettings, IBotIntents botIntents, ILoggerFactory loggerFactory)
        {
            _userState = userState.CheckNullReference();
            _conversationState = conversationState.CheckNullReference();
            _botSettings = botSettings.CheckNullReference();
            _botIntents = botIntents.CheckNullReference();

            // Verify LUIS configuration.
            if (!services.CheckNullReference().LuisServices.ContainsKey(botSettings.LuisConfiguration))
            {
                throw new InvalidOperationException($"The bot configuration does not contain a service type of `luis` with the id `{botSettings.LuisConfiguration}`.");
            }
            _luisRecognizer = services.LuisServices[botSettings.LuisConfiguration];


            _entityStateAccessor = _userState.CreateProperty<EntityState>(nameof(EntityState));
            var dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
            _dialogSet = new BotDialogSet(dialogStateAccessor, _entityStateAccessor, loggerFactory);

            _logger = loggerFactory.CheckNullReference().CreateLogger<MultiTurnBot>();
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;
            var dialogContext = await _dialogSet.CreateDialogContextAsync(turnContext);

            if (activity.Type == ActivityTypes.Message)
            {

                var luisResults = await _luisRecognizer.RecognizeAsync(dialogContext.Context, cancellationToken);
                var topIntent = GetTopIntent(luisResults);

                await UpdateEntityState(luisResults, turnContext);

                // Handle conversation interrupts first.
                var interrupted = await IsTurnInterruptedAsync(dialogContext, topIntent);
                if (interrupted)
                {
                    await ExitTurnAsync(turnContext);
                    return;
                }

                var dialogResult = await dialogContext.ContinueDialogAsync(cancellationToken);

                if (!dialogContext.Context.Responded)
                {
                    // examine results from active dialog
                    switch (dialogResult.Status)
                    {
                        case DialogTurnStatus.Empty:
                            if (!string.IsNullOrEmpty(topIntent.DialogName))
                            {
                                await dialogContext.BeginDialogAsync(topIntent.DialogName);
                            }
                            else
                            {
                                await dialogContext.Context.SendActivityAsync(_botIntents.FallbackActivity);
                            }
                            break;

                        case DialogTurnStatus.Waiting:
                            // The active dialog is waiting for a response from the user, so do nothing.
                            break;

                        case DialogTurnStatus.Complete:
                            await dialogContext.EndDialogAsync();
                            break;

                        default:
                            await dialogContext.CancelAllDialogsAsync();
                            break;
                    }
                }
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                await SendWelcomeMessageAsync(activity, dialogContext);
            }

            await ExitTurnAsync(turnContext);
        }

        protected async Task SendWelcomeMessageAsync(Activity activity, DialogContext dialogContext)
        {
            if (activity.MembersAdded != null)
            {
                // Iterate over all new members added to the conversation.
                foreach (var member in activity.MembersAdded)
                {
                    // Greet anyone that was not the target (recipient) of this message.
                    if (member.Id != activity.Recipient.Id)
                    {
                        await dialogContext.Context.SendActivityAsync(_botIntents.WelcomeActivity);
                    }
                }
            }
        }

        protected async Task ExitTurnAsync(ITurnContext turnContext)
        {
            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }

        protected Intent GetTopIntent(RecognizerResult luisResult)
        {
            if (luisResult == null)
            {
                return _botIntents.DefaultIntent;
            }

            var topScoringIntent = luisResult.GetTopScoringIntent();
            if (topScoringIntent.score >= _botIntents.IntentTriggerThreshold)
            {
                var topIntent = topScoringIntent.intent;
                if (_botIntents.Intents.ContainsKey(topIntent))
                {
                    return _botIntents.Intents[topIntent];
                }
                else
                {
                    _logger.LogError($"Fail to find intent {topIntent} in bot definition.");
                }
            }
            return _botIntents.DefaultIntent;
        }

        protected async Task<bool> IsTurnInterruptedAsync(DialogContext dialogContext, Intent topIntent)
        {
            // See if there are any conversation interrupts we need to handle.
            if (topIntent.Type == IntentType.Termination)
            {
                return await TerminateIntentAsync(dialogContext, topIntent as TerminationIntent);
            }

            if (topIntent.Type == IntentType.Detour)
            {
                return await DetourIntentAsync(dialogContext, topIntent as DetourIntent);
            }

            return false;
        }

        protected async Task<bool> TerminateIntentAsync(DialogContext dialogContext, TerminationIntent intent)
        {
            IActivity[] activities = intent.TerminationFailActivities;
            if (dialogContext.ActiveDialog != null)
            {
                await dialogContext.CancelAllDialogsAsync();
                activities = intent.TerminationSuccessActivities;
            }

            foreach (var activity in activities)
            {
                await dialogContext.Context.SendActivityAsync(activity);
            }

            return true;
        }

        protected async Task<bool> DetourIntentAsync(DialogContext dialogContext, DetourIntent intent)
        {
            IActivity[] activities = intent.DetourActivities;

            foreach (var activity in activities)
            {
                await dialogContext.Context.SendActivityAsync(activity);
            }

            if (dialogContext.ActiveDialog != null)
            {
                await dialogContext.RepromptDialogAsync();
            }

            return true;
        }

        protected async Task UpdateEntityState(RecognizerResult luisResult, ITurnContext turnContext)
        {
            try
            {
                if (luisResult.Entities != null && luisResult.Entities.HasValues)
                {
                    var entityState = await _entityStateAccessor.GetAsync(turnContext, () => new EntityState());
                    var entities = luisResult.Entities;

                    foreach (var entityMapping in _botSettings.Entities)
                    {
                        var luisEntity = entityMapping.Value;
                        foreach (var entity in luisEntity.Patterns)
                        {
                            if (entities[entity] != null)
                            {
                                entityState.AddOrUpdate(entityMapping.Key, luisEntity.PostProcess((string)entities[entity][0]));
                            }
                        }
                    }

                    await _entityStateAccessor.SetAsync(turnContext, entityState);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(UpdateEntityState)} failed with exception {e.ToString()}");
            }
        }
    }
}
