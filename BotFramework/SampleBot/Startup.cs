// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.3.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using UiPath.ChatbotSamples.BotFramework.Actions;
using UiPath.ChatbotSamples.BotFramework.Bot.Common;
using UiPath.ChatbotSamples.BotFramework.Common;
using UiPath.ChatbotSamples.BotFramework.Dialogs;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Auth;

namespace UiPath.ChatbotSamples.BotFramework.Bot
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Add Bot configuration.
            var botConfig = LoadBotConfiguration();
            services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot configuration file could not be loaded."));

            // Add BotServices singleton.
            // Create the connected services from .bot file.
            services.AddSingleton(sp => new BotServices(botConfig));

            // Memory Storage is for local bot debugging only. When the bot
            // is restarted, everything stored in memory will be gone.
            // For production bots use the Azure Blob or
            // Azure CosmosDB storage providers.
            IStorage dataStore = new MemoryStorage();

            // Create and add conversation state.
            var conversationState = new ConversationState(dataStore);
            services.AddSingleton(conversationState);

            var userState = new UserState(dataStore);
            services.AddSingleton(userState);
            services.AddOptions();

            //Inject settings and sericies.
            services.Configure<OrchestratorSettings>(Configuration.GetSection("OrchestratorSettings"));
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IOrchestratorClient, OrchestratorClient.OrchestratorClient>();
            services.AddSingleton<IMyRpaClient, MyRpaClient>();

            // Inject bot.
            services.AddBot<MyBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }

        private BotConfiguration LoadBotConfiguration()
        {
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;
            if (!File.Exists(botFilePath))
            {
                throw new FileNotFoundException($"The .bot configuration file was not found. botFilePath: {botFilePath}");
            }

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            BotConfiguration botConfig = null;
            try
            {
                botConfig = BotConfiguration.Load(botFilePath, secretKey);
            }
            catch
            {
                var msg = @"Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.
                          - You can find the botFilePath and botFileSecret in the Azure App Service application settings.
                          - If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.
                          - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
                          ";
                throw new InvalidOperationException(msg);
            }

            return botConfig;
        }
    }
}
