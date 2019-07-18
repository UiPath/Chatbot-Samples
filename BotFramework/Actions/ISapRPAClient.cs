using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Actions.Models;

namespace UiPath.ChatbotSamples.BotFramework.Actions
{
    public interface ISapRpaClient
    {
        Task<GetItemsOutput> GetItemsAsync(GetItemsInput input);

        Task<CreatePurchaseOrderOutput> CreatePurchaseOrderAsync(CreatePurchaseOrderInput input);

        Task<CreateSalesOrderOutput> CreateSalesOrderAsync(CreateSalesOrderInput input);

        Task<CancelOrderOutput> CancelOrderAsync(CancelOrderInput input);
    }
}
