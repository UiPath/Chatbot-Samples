using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels;
using UiPath.ChatbotSamples.BotFramework.Test.Common;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Test
{
    public class ClientTest
    {
        private readonly OrchestratorSettingOption _settingOption = new OrchestratorSettingOption(
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

        private readonly ITokenService _mockTokenService = MockHelper.CreateMockTokenService().Object;

        private readonly OrchestratorClient _orchestratorClient;

        public ClientTest()
        {
            _orchestratorClient = new OrchestratorClient(_mockTokenService, _settingOption);
        }

        [Fact]
        public void ThrowsIfFailedToStart()
        {
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.NotFound);
            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);
            var httpClient = new HttpClient(basicAuthHeadhandler);

            Assert.Throws<Exception>(() => _orchestratorClient.StartJobAsync(new StartJobInfo(), httpClient).GetAwaiter().GetResult());
        }

        [Fact]
        public void SuccessIfStart()
        {
            var mockResponse = new ODataList<StartJobResponse>()
            {
                Value = new StartJobResponse[] {
                    new StartJobResponse()
                    {
                        Id = "1",
                        Key = Guid.NewGuid().ToString(),
                        State = "Started",
                    },
                },
            };
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.OK, Utils.GetPostBody(mockResponse));

            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler);

            var response = _orchestratorClient.StartJobAsync(new StartJobInfo(), httpClient).GetAwaiter().GetResult();
            Assert.Equal(mockResponse.Value[0].Key, response.Key);
        }

        [Fact]
        public void ThrowsIfJobFaulted()
        {
            var mockResponse = new ODataList<StartJobResponse>()
            {
                Value = new StartJobResponse[] {
                    new StartJobResponse()
                    {
                        Id = "1",
                        Key = Guid.NewGuid().ToString(),
                        State = "Faulted",
                    },
                },
            };
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.OK, Utils.GetPostBody(mockResponse));

            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler);

            Assert.Throws<Exception>(() => _orchestratorClient.WaitForJobCompletionAsync("1", httpClient).GetAwaiter().GetResult());
        }

        [Fact]
        public void ThrowsIfJobStopped()
        {
            var mockResponse = new ODataList<StartJobResponse>()
            {
                Value = new StartJobResponse[] {
                    new StartJobResponse()
                    {
                        Id = "1",
                        Key = Guid.NewGuid().ToString(),
                        State = "Stopped",
                    },
                },
            };
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.OK, Utils.GetPostBody(mockResponse));

            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler);

            Assert.Throws<Exception>(() => _orchestratorClient.WaitForJobCompletionAsync("1", httpClient).GetAwaiter().GetResult());
        }

        [Fact]
        public void ThrowsIfJobTimeOut()
        {
            var mockResponse = new ODataList<StartJobResponse>()
            {
                Value = new StartJobResponse[] {
                    new StartJobResponse()
                    {
                        Id = "1",
                        Key = Guid.NewGuid().ToString(),
                        State = "Started",
                    },
                },
            };
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.OK, Utils.GetPostBody(mockResponse));

            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler);

            Assert.Throws<Exception>(() => _orchestratorClient.WaitForJobCompletionAsync("1", httpClient).GetAwaiter().GetResult());

            mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(_settingOption.CurrentValue.StatusCheckMaxRetry),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void SuccessIfJobCompletedSuccess()
        {
            var mockResponse = new ODataList<StartJobResponse>()
            {
                Value = new StartJobResponse[] {
                    new StartJobResponse()
                    {
                        Id = "1",
                        Key = Guid.NewGuid().ToString(),
                        State = "Successful",
                    },
                },
            };
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.OK, Utils.GetPostBody(mockResponse));

            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler);

            _orchestratorClient.WaitForJobCompletionAsync("1", httpClient).GetAwaiter().GetResult();
        }

        [Fact]
        public void ThrowsIfFailedToGetDetail()
        {
            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.NotFound);
            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);
            var httpClient = new HttpClient(basicAuthHeadhandler);

            Assert.Throws<Exception>(() => _orchestratorClient.GetJobDetailAsync("1", httpClient).GetAwaiter().GetResult());
        }

        [Fact]
        public void SuccessIfGetJobDetail()
        {
            var mockResponse = new StartJobResponse()
            {
                Id = "1",
                Key = Guid.NewGuid().ToString(),
                State = "Successful",
                OutputArguments = "{\"key\":\"value\"}", 
            };

            var mockMessageHandler = MockHelper.CreateMockMessagHandler(HttpStatusCode.OK, Utils.GetPostBody(mockResponse));

            var basicAuthHeadhandler = new BasicAuthHeadHandler(_mockTokenService, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler);

            var response = _orchestratorClient.GetJobDetailAsync("1", httpClient).GetAwaiter().GetResult();
            Assert.Equal(mockResponse.Key, response.Key);
        }
    }
}
