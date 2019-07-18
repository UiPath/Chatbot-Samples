using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient
{
    public static class Utils
    {
        public static string GetBasicAuthUrl(string baseUrl)
        {
            return ConcatUrl(baseUrl, _basicAuthUrl);
        }

        public static string CloudAuthUrl => "https://account.uipath.com/oauth/token";

        public static string GetStartjobUrl(string baseUrl)
        {
            return ConcatUrl(baseUrl, _startJobUrl);
        }

        public static string GetJobStatusUrl(string baseUrl, string jobKey)
        {
            return ConcatUrl(baseUrl, "odata/Jobs?$top=3&$filter=Key eq " + jobKey);
        }

        public static string GetJobDetailUrl(string baseUrl, string jobId)
        {
            return ConcatUrl(baseUrl, "odata/Jobs(" + jobId + @")");
        }

        public static StringContent GetPostBody<T>(T t)
        {
            return new StringContent(SerializePostBody(t), Encoding.UTF8, "application/json");
        }

        private static string SerializePostBody<T>(T t)
        {
            return JsonConvert.SerializeObject(t, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private static string _basicAuthUrl => "api/account/authenticate";

        private static string _startJobUrl => "odata/Jobs/UiPath.Server.Configuration.OData.StartJobs";

        private static string ConcatUrl(string baseUrl, string routing)
        {
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(routing))
            {
                throw new InvalidOperationException("cannot concat url with null or empty string");
            }
            var delimiter = '/';

            return $"{baseUrl.TrimEnd(delimiter)}{delimiter}{routing.TrimStart(delimiter)}";
        }
    }
}
