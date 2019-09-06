using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UiPath.ChatbotSamples.BotFramework.Actions.Models;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels;
using UiPath.ChatbotSamples.BotFramework.Test.Common;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.Actions.Test
{
    public class MyRpaClientTest
    {
        private readonly OrchestratorSettingOption _invalidSetting = new OrchestratorSettingOption(
            new OrchestratorSettings()
            {
                AuthMode = "Cloud",
                RefreshToken = "randometoken",
                ServiceInstanceLogicalName = "test",
                AccountLogicalName = "test",
                BaseUrl = "Https://platform.uipath.com",
                Strategy = "Specific",
                RobotIds = new long[] { 1, 2, 3 },
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            });

        private readonly OrchestratorSettingOption _validSetting = new OrchestratorSettingOption(
            new OrchestratorSettings()
            {
                AuthMode = "Cloud",
                RefreshToken = "randometoken",
                ServiceInstanceLogicalName = "test",
                AccountLogicalName = "test",
                BaseUrl = "Https://platform.uipath.com",
                Strategy = "Specific",
                RobotIds = new long[] { 1, 2, 3 },
                ProcessKeys = new ProcessKey[] {
                    new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "CreatePurchaseOrder" },
                    new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "CreateSalesOrder" },
                    new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "GetItems" },
                    new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "CancelOrder" },
                },
            });

        [Fact]
        public void ThrowIfCannotFindProcessKey()
        {
            var orchestratorClient = new Mock<IOrchestratorClient>();

            var client = new MyRpaClient(orchestratorClient.Object, _invalidSetting);
            Assert.Throws<InvalidOperationException>(() => client.CreatePurchaseOrderAsync(new CreatePurchaseOrderInput()).GetAwaiter().GetResult());
        }

        [Fact]
        public void CreatePurchaseOrderThrowIfNoOutput()
        {
            CreatePurchaseOrderOutput mockOutput = null;
            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            Assert.Throws<ArgumentNullException>(() => client.CreatePurchaseOrderAsync(new CreatePurchaseOrderInput()).GetAwaiter().GetResult());
        }

        [Fact]
        public void CreatePurchaseOrderSuccess()
        {
            var mockOutput = new CreatePurchaseOrderOutput()
            {
                PurchaseOrderId = "123456",
            };

            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            var output = client.CreatePurchaseOrderAsync(new CreatePurchaseOrderInput()).GetAwaiter().GetResult();
            Assert.Equal(mockOutput.PurchaseOrderId, output.PurchaseOrderId);
        }

        [Fact]
        public void CreateSalesOrderThrowIfNoOutput()
        {
            CreateSalesOrderOutput mockOutput = null;
            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            Assert.Throws<ArgumentNullException>(() => client.CreateSalesOrderAsync(new CreateSalesOrderInput()).GetAwaiter().GetResult());
        }

        [Fact]
        public void CreateSalesOrderSuccess()
        {
            var mockOutput = new CreateSalesOrderOutput()
            {
                OrderId = "123456",
                DeliveryDate = DateTime.Today.AddDays(3),
            };

            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            var output = client.CreateSalesOrderAsync(new CreateSalesOrderInput()).GetAwaiter().GetResult();
            Assert.Equal(mockOutput.OrderId, output.OrderId);
            Assert.Equal(mockOutput.DeliveryDate.ToShortDateString(), output.DeliveryDate.ToShortDateString());
        }

        [Fact]
        public void GetItemsThrowIfNoOutput()
        {
            GetItemsOutput mockOutput = null;
            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            Assert.Throws<ArgumentNullException>(() => client.GetItemsAsync(new GetItemsInput()).GetAwaiter().GetResult());
        }

        [Fact]
        public void GetItemsSuccess()
        {
            var mockOutput = new GetItemsOutputInternal()
            {
                DeliveryDate = new string[] 
                {
                    DateTime.Today.AddDays(2).ToShortDateString(),
                    DateTime.Today.AddDays(10).ToShortDateString()
                },
                ItemId = new string[] { "1234", "5678" },
                OrderId = new string[] { "7890", "1357" },
                ItemName = new string[] { "Name1", "Name2" },
                Quantity = new string[] { "1", "2" },
                DeliveryStatus = new string[] { "out of stock", "delivered" },
            };

            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            var output = client.GetItemsAsync(new GetItemsInput()).GetAwaiter().GetResult();
            Assert.Equal(mockOutput.ItemId.Length, output.Items.Length);
            Assert.Equal(mockOutput.ItemId[0], output.Items[0].ItemId);
        }

        [Fact]
        public void CancelOrderThrowIfNoOutput()
        {
            CancelOrderOutput mockOutput = null;
            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            Assert.Throws<ArgumentNullException>(() => client.CancelOrderAsync(new CancelOrderInput()).GetAwaiter().GetResult());
        }

        [Fact]
        public void CancelOrderSuccess()
        {
            var mockOutput = new CancelOrderOutput()
            {
                ReturnLabelLocation = "randome locaion",
            };

            var mockResponse = CreateJobResponse(mockOutput);

            var client = new MyRpaClient(GetMockOrchestratorClient(mockResponse), _validSetting);
            var output = client.CancelOrderAsync(new CancelOrderInput()).GetAwaiter().GetResult();
            Assert.Equal(mockOutput.ReturnLabelLocation, output.ReturnLabelLocation);
        }

        private IOrchestratorClient GetMockOrchestratorClient(StartJobResponse mockResponse)
        {
            var orchestratorClient = new Mock<IOrchestratorClient>();
            orchestratorClient
                .Setup(c => c.ExecuteJobAsync(It.IsAny<StartJobInfo>()))
                .ReturnsAsync(mockResponse);
            return orchestratorClient.Object;
        }

        private StartJobResponse CreateJobResponse<T>(T t)
        {
            return new StartJobResponse()
            {
                Id = "random",
                Key = "random",
                State = "random",
                OutputArguments = t == null? null : JsonConvert.SerializeObject(t),
            };
        }
    }
}
