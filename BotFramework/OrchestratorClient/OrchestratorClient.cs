using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient
{
    public class OrchestratorClient : IOrchestratorClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly int _statusCheckInterval;
        private readonly int _statusMaxRetry;

        public OrchestratorClient(ITokenService tokenService, IOptionsMonitor<OrchestratorSettings> orchestratorSettingsAccessor)
        {
            var orchestratorSettings = orchestratorSettingsAccessor.CheckNullReference().CurrentValue;
            if (!orchestratorSettings.Validate())
            {
                throw new ArgumentException("Orchestrator setting invalid");
            }

            if (orchestratorSettings.AuthMode.OrdinalEquals("Basic"))
            {
                _client = new HttpClient(new BasicAuthHeadHandler(tokenService.CheckNullReference()));
            }
            else
            {
                _client = new HttpClient(new CloudAuthHeadHandler(tokenService.CheckNullReference(), orchestratorSettings.ServiceInstanceLogicalName));
            }

            _baseUrl = orchestratorSettings.CheckNullReference().BaseUrl;
            _statusCheckInterval = orchestratorSettings.StatusCheckInterval;
            _statusMaxRetry = orchestratorSettings.StatusCheckMaxRetry;
        }

        public async Task<StartJobResponse> ExecuteJobAsync(StartJobInfo jobInfo)
        {
            var jobResponse = await StartJobAsync(jobInfo);
            await WaitForJobCompletionAsync(jobResponse.Key);
            return await GetJobDetailAsync(jobResponse.Id);
        }

        // Depending on user scenario, if the robot is very busy and start job may fail. So a retry will need to be added here.
        public async Task<StartJobResponse> StartJobAsync(StartJobInfo jobInfo, HttpClient client = null)
        {
            StartJobBody body = new StartJobBody() { startInfo = jobInfo };
            var startJobResponseList = await HttpCallAsync<ODataList<StartJobResponse>>(Utils.GetStartjobUrl(_baseUrl), HttpMethod.Post, Utils.GetPostBody(body), client);
            return startJobResponseList.Value[0];
        }

        public async Task WaitForJobCompletionAsync(string jobKey, HttpClient client = null)
        {
            int count = 0;
            while (count++ < _statusMaxRetry)
            {
                await Task.Delay(_statusCheckInterval);

                var jobs = await HttpCallAsync<ODataList<StartJobResponse>>(Utils.GetJobStatusUrl(_baseUrl, jobKey), HttpMethod.Get, null, client);
                if (jobs?.Value?.Length == null)
                {
                    continue;
                }

                var job = jobs.Value[0];

                if (job.State.OrdinalEquals("Successful"))
                {
                    return;
                }
                if (job.State.OrdinalEquals("Faulted") || job.State.OrdinalEquals("Stopped"))
                {
                    throw new Exception($"Job {jobKey} completed with {job.State} state.");
                }
            }
            throw new Exception($"Job {jobKey} timedout after {_statusCheckInterval * _statusMaxRetry} ms.");
        }

        public async Task<StartJobResponse> GetJobDetailAsync(string jobId, HttpClient client = null)
        {
            return await HttpCallAsync<StartJobResponse>(Utils.GetJobDetailUrl(_baseUrl, jobId), HttpMethod.Get, null, client);
        }

        private void EnsureSuccessStatus(HttpResponseMessage response)
        {
            if (response?.StatusCode != HttpStatusCode.OK && response?.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception("Orchestrator call failed");
            }
        }

        private async Task<T> HttpCallAsync<T>(string url, HttpMethod httpMethod, HttpContent body = null, HttpClient client = null) where T: class
        {
            client = client ?? _client;

            HttpResponseMessage response = null;

            if (httpMethod == HttpMethod.Get)
            {
                response = await client.GetAsync(url, new CancellationToken());
            }
            else if (httpMethod == HttpMethod.Post)
            {
                response = await client.PostAsync(url, body, new CancellationToken());
            }

            EnsureSuccessStatus(response);

            return await (response).Content.ReadAsAsync<T>();
        }
    }
}
