using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth
{
    public class TokenService : ITokenService
    {
        private readonly BasicAuthSettings _basicAuthSettings;
        private readonly CloudAuthSettings _cloudAuthSettings;
        private readonly string _baseUrl;

        public TokenService(IOptionsMonitor<OrchestratorSettings> orchestratorSettingAccessor)
        {
            var orchestratorSettings = orchestratorSettingAccessor.CheckNullReference().CurrentValue;

            _basicAuthSettings = new BasicAuthSettings()
            {
                TenancyName = orchestratorSettings.TenancyName,
                UsernameOrEmailAddress = orchestratorSettings.UsernameOrEmailAddress,
                Password = orchestratorSettings.Password,
            };

            _cloudAuthSettings = new CloudAuthSettings()
            {
                refresh_token = orchestratorSettings.RefreshToken,
            };

            _baseUrl = orchestratorSettings.BaseUrl;
        }

        public async Task<BasicAuthResponse> BasicAuthenticateAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var tokenResponse = await client.PostAsync(Utils.GetBasicAuthUrl(_baseUrl), Utils.GetPostBody(_basicAuthSettings), new CancellationToken());
                return await tokenResponse.Content.ReadAsAsync<BasicAuthResponse>();
            }
        }

        public async Task<CloudAuthResponse> CloudAuthenticateAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var tokenResponse = await client.PostAsync(Utils.CloudAuthUrl, Utils.GetPostBody(_cloudAuthSettings), new CancellationToken());
                return await tokenResponse.Content.ReadAsAsync<CloudAuthResponse>();
            }
        }
    }
}
