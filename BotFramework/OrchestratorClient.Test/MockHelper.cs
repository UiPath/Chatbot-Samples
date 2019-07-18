using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Test
{
    public sealed class MockHelper
    {
        public static string Token => "Test Token";

        public static Mock<ITokenService> CreateMockTokenService()
        {
            var mockTokenService = new Mock<ITokenService>();

            mockTokenService.Setup(t => t.BasicAuthenticateAsync())
                .Returns(Task.FromResult(new BasicAuthResponse() { Result = Token, }));

            mockTokenService.Setup(t => t.CloudAuthenticateAsync())
                .Returns(Task.FromResult(new CloudAuthResponse() { access_token = Token, }));

            return mockTokenService;
        }

        public static Mock<HttpMessageHandler> CreateMockMessagHandler(HttpStatusCode statusCode = HttpStatusCode.OK, HttpContent response = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = statusCode,
                   Content = response,
               })
               .Verifiable();

            return handlerMock;
        }
    }
}
