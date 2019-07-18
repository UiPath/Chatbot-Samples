using System;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Test
{
    public class UtilsTest
    {
        [Fact]
        public void GetValidUrlWithTrailingBaseUrl()
        {
            var baseUrl = "http://localhost/";
            var jobKey = "123";
            Assert.True(Uri.IsWellFormedUriString(Utils.GetBasicAuthUrl(baseUrl), UriKind.Absolute));
            Assert.True(Uri.IsWellFormedUriString(Utils.GetJobDetailUrl(baseUrl, jobKey), UriKind.Absolute));
            // job status url contains parameter, need to remove those before check
            var jobStatusUrl = Utils.GetJobStatusUrl(baseUrl, jobKey);
            Assert.True(Uri.IsWellFormedUriString(jobStatusUrl.Substring(0, jobStatusUrl.IndexOf('$')), UriKind.Absolute));
            Assert.True(Uri.IsWellFormedUriString(Utils.GetStartjobUrl(baseUrl), UriKind.Absolute));
        }

        [Fact]
        public void GetValidUrlWithNoneTrailingBaseUrl()
        {
            var baseUrl = "http://localhost";
            var jobKey = "123";

            var authUrl = Utils.GetBasicAuthUrl(baseUrl);
            var jobDetailUrl = Utils.GetJobDetailUrl(baseUrl, jobKey);
            // job status url contains parameter, need to remove those before check
            var jobStatusUrl = Utils.GetJobStatusUrl(baseUrl, jobKey);
            var startJobUrl = Utils.GetStartjobUrl(baseUrl);

            Assert.True(Uri.IsWellFormedUriString(authUrl, UriKind.Absolute));
            Assert.True(Uri.IsWellFormedUriString(jobDetailUrl, UriKind.Absolute));
            Assert.True(Uri.IsWellFormedUriString(jobStatusUrl.Substring(0, jobStatusUrl.IndexOf('$')), UriKind.Absolute));
            Assert.True(Uri.IsWellFormedUriString(startJobUrl, UriKind.Absolute));

            var trailedBaseUrl = $"{baseUrl}/";
            Assert.StartsWith(trailedBaseUrl, authUrl);
            Assert.StartsWith(trailedBaseUrl, jobDetailUrl);
            Assert.StartsWith(trailedBaseUrl, jobStatusUrl);
            Assert.StartsWith(trailedBaseUrl, startJobUrl);
        }
    }
}
