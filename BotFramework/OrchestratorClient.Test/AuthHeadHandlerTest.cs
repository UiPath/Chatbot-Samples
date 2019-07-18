using Moq;
using Moq.Protected;
using System.Linq;
using System.Net.Http;
using System.Threading;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Test
{
    public class AuthHeadHandlerTest
    {
        [Fact]
        public void BasicAuthShouldAddTokenHead()
        {
            var mockTokenService = MockHelper.CreateMockTokenService();
            var mockMessageHandler = MockHelper.CreateMockMessagHandler();

            var basicAuthHeadhandler = new BasicAuthHeadHandler(mockTokenService.Object, mockMessageHandler.Object);

            var httpClient = new HttpClient(basicAuthHeadhandler)
            {
                BaseAddress = new System.Uri("http://localhost"),
            };

            httpClient.GetAsync(httpClient.BaseAddress).GetAwaiter().GetResult();

            mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Headers.Contains("Authorization") &&
                    req.Headers.GetValues("Authorization").FirstOrDefault() == $"Bearer {MockHelper.Token}"),
                ItExpr.IsAny<CancellationToken>());

        }

        [Fact]
        public void CloudAuthShouldAddTokenHead()
        {
            var mockTokenService = MockHelper.CreateMockTokenService();
            var mockMessageHandler = MockHelper.CreateMockMessagHandler();
            const string serviceInstanceLogicalName = "TestLogicalName";
            var cloudAuthHeadhandler = new CloudAuthHeadHandler(mockTokenService.Object, serviceInstanceLogicalName, mockMessageHandler.Object);

            var httpClient = new HttpClient(cloudAuthHeadhandler)
            {
                BaseAddress = new System.Uri("http://localhost"),
            };

            httpClient.GetAsync(httpClient.BaseAddress).GetAwaiter().GetResult();

            mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // we expected a single external request
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Contains("Authorization") &&
                    req.Headers.GetValues("Authorization").FirstOrDefault() == $"Bearer {MockHelper.Token}" &&
                    req.Headers.Contains("X-UIPATH-TenantName") &&
                    req.Headers.GetValues("X-UIPATH-TenantName").FirstOrDefault() == serviceInstanceLogicalName),
                ItExpr.IsAny<CancellationToken>());

        }
    }
}
