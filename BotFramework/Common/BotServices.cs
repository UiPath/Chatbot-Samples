using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Configuration;
using System;
using System.Collections.Generic;

namespace UiPath.ChatbotSamples.BotFramework.Common
{
    /// <summary>
    /// Represents references to external services.
    ///
    /// For example, LUIS services are kept here as a singleton.  This external service is configured
    /// using the <see cref="BotConfiguration"/> class.
    /// </summary>
    //  See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1
    //  for more information regarding dependency injection
    //  See https://www.luis.ai/home" for more information regarding language understanding using LUIS
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// <param name="botConfiguration">A dictionary of named <see cref="BotConfiguration"/> instances for usage within the bot.</param>
        /// </summary>
        public BotServices(BotConfiguration botConfiguration)
        {
            foreach (var service in botConfiguration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            var luis = (LuisService)service;
                            EnsureNotNull(luis, ServiceTypes.Luis);

                            var app = new LuisApplication(luis.AppId, luis.AuthoringKey, luis.GetEndpoint());
                            var recognizer = new LuisRecognizer(app);
                            LuisServices.Add(luis.Name, recognizer);
                            break;
                        }
                    case ServiceTypes.Endpoint:
                        {
                            var endPoint = (EndpointService)service;
                            EnsureNotNull(endPoint, ServiceTypes.Endpoint);
                            EndpointServices.Add(endPoint.Name, endPoint);
                            break;
                        }
                }
            }

            if (EndpointServices.Count == 0)
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint.");
            }
        }

        /// <summary>
        /// Gets the set of LUIS Services used.
        /// Given there can be multiple <see cref="LuisRecognizer"/> services used in a single bot,
        /// LuisServices is represented as a dictionary.  This is also modeled in the
        /// ".bot" file since the elements are named.
        /// <remarks>The LUIS services collection should not be modified while the bot is running.</remarks>
        /// <value>
        /// A <see cref="LuisRecognizer"/> client instance created based on configuration in the .bot file.
        /// </value>
        /// </summary>
        public Dictionary<string, LuisRecognizer> LuisServices { get; } = new Dictionary<string, LuisRecognizer>();

        /// <summary>
        /// Gets the set of endpoint services used.
        /// Given there can be multiple endpoints for different environments,
        /// EndpointServices is represented as a dictionary.
        /// </summary>
        public Dictionary<string, EndpointService> EndpointServices { get; } = new Dictionary<string, EndpointService>();

        private void EnsureNotNull<T>(T service, string serviceType)
        {
            if (service == null)
            {
                throw new InvalidCastException($"The {serviceType} service is not configured correctly in your .bot file.");
            }
        }
    }
}
