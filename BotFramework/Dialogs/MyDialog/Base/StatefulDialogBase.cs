using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Common.Models;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.MyDialog.Base
{
    public class StatefulDialogBase: DialogBase
    {
        private readonly IStatePropertyAccessor<EntityState> _entityStateAccessor;

        public StatefulDialogBase(string dialogId, IStatePropertyAccessor<EntityState> entityStateAccessor)
            : base(dialogId)
        {
            _entityStateAccessor = entityStateAccessor;
        }

        protected async Task<EntityState> GetEntityStateAsync(WaterfallStepContext stepContext)
        {
            var entityState = await _entityStateAccessor.GetAsync(stepContext.Context, () => null);
            if (entityState == null)
            {
                entityState = new EntityState();
                await _entityStateAccessor.SetAsync(stepContext.Context, entityState);
            }
            return entityState;
        }

        protected async Task<string> TryGetEntityValueAsync(WaterfallStepContext stepContext, string key)
        {
            var entityState = await GetEntityStateAsync(stepContext);
            return entityState.TryGet(key);
        }
    }
}
