using Microsoft.Bot.Builder.Dialogs;

namespace UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog.Base
{
    public class DialogBase: ComponentDialog
    {
        private readonly string _dialogId;

        protected DialogBase(string dialogId)
            : base(dialogId)
        {
            _dialogId = dialogId;
        }

        protected string GetWaterFallDialogId(string id)
        {
            return $"WaterFall{id}";
        }
    }
}
