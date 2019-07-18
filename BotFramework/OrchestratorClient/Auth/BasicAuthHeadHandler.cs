using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth
{
    public class BasicAuthHeadHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;

        public BasicAuthHeadHandler(ITokenService tokenService)
            : base(new HttpClientHandler())
        {
            _tokenService = tokenService;
        }

        public BasicAuthHeadHandler(ITokenService tokenService, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.BasicAuthenticateAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
