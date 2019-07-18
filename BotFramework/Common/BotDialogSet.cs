using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Common.Models;

namespace UiPath.ChatbotSamples.BotFramework.Common
{
    public class BotDialogSet
    {
        private DialogSet _dialogs;

        public BotDialogSet(IStatePropertyAccessor<DialogState> dialogStateAccessor, IStatePropertyAccessor<EntityState> userProfileStateAccessor, ILoggerFactory loggerFactory)
        {
            _dialogs = new DialogSet(dialogStateAccessor);
        }

        public void AddDialogToSet(Dialog dialog)
        {
            _dialogs.Add(dialog);
        }

        public async Task<DialogContext> CreateDialogContextAsync(ITurnContext turnContext)
        {
            return await _dialogs.CreateContextAsync(turnContext);
        }
    }
}
