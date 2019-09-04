using System;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Actions.Models;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.Actions
{
    public class MyRpaClientMock : IMyRpaClient
    {
        public async Task<GetItemsOutput> GetItemsAsync(GetItemsInput input)
        {
            if (input.CustomerId.OrdinalEquals("123456"))
            {
                return new GetItemsOutput()
                {
                    Items = new Item[]
                    {
                        new Item()
                        {
                            DeliveryDate = DateTime.Today.AddDays(2),
                            ItemId = "35678",
                            OrderId = "45436",
                            ItemName = "wireless speaker",
                            Quantity = "1",
                            DeliveryStatus = "out of stock",
                        },
                        new Item()
                        {
                            DeliveryDate = DateTime.Today.AddDays(10),
                            ItemId = "35678",
                            OrderId = "45436",
                            ItemName = "TV",
                            Quantity = "1",
                            DeliveryStatus = "out of stock",
                        },
                        new Item()
                        {
                            DeliveryDate = DateTime.Today.AddDays(-1),
                            ItemId = "123",
                            OrderId = "5645",
                            ItemName = "monitor",
                            Quantity = "1",
                            DeliveryStatus = "delivered",
                        },
                        new Item()
                        {
                            DeliveryDate = DateTime.Today.AddDays(-5),
                            ItemId = "123",
                            OrderId = "5645",
                            ItemName = "headphone",
                            Quantity = "1",
                            DeliveryStatus = "delivered",
                        },
                    },
                };
            }
            else
            {
                return new GetItemsOutput();
            }
        }

        public async Task<CreatePurchaseOrderOutput> CreatePurchaseOrderAsync(CreatePurchaseOrderInput input)
        {
            return new CreatePurchaseOrderOutput()
            {
                PurchaseOrderId = "12345",
            };
        }

        public async Task<CreateSalesOrderOutput> CreateSalesOrderAsync(CreateSalesOrderInput input)
        {
            return new CreateSalesOrderOutput()
            {
                DeliveryDate = DateTime.Today.AddDays(7),
                OrderId = "45678",
            };
        }

        public async Task<CancelOrderOutput> CancelOrderAsync(CancelOrderInput input)
        {
            return new CancelOrderOutput()
            {
                ReturnLabelLocation = "http://returnurl/123",
            };
        }
    }
}
