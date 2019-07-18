using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth
{
    public class CloudAuthHeadHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;
        private readonly string _serviceInstanceLogicalName;

        public CloudAuthHeadHandler(ITokenService tokenService, string serviceInstanceLogicalName)
            : base(new HttpClientHandler())
        {
            _tokenService = tokenService;
            _serviceInstanceLogicalName = serviceInstanceLogicalName;
        }

        public CloudAuthHeadHandler(ITokenService tokenService, string serviceInstanceLogicalName, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _tokenService = tokenService;
            _serviceInstanceLogicalName = serviceInstanceLogicalName;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.CloudAuthenticateAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
            request.Headers.Add("X-UIPATH-TenantName", _serviceInstanceLogicalName);


            return await base.SendAsync(request, cancellationToken);
        }
    }
}
