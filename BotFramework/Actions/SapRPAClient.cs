using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UiPath.ChatbotSamples.BotFramework.Actions.Models;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient;
using UiPath.ChatbotSamples.BotFramework.OrchestratorClient.JobModels;
using UiPath.ChatbotSamples.BotFramework.Utils;

namespace UiPath.ChatbotSamples.BotFramework.Actions
{
    public class SapRpaClient: ISapRpaClient
    {
        private readonly IOrchestratorClient _orchestratorClient;
        private readonly OrchestratorSettings _orchestratorSettings;

        public SapRpaClient(IOrchestratorClient orchestratorClient, IOptionsMonitor<OrchestratorSettings> orchestratorSettingsAccessor)
        {
            _orchestratorClient = orchestratorClient.CheckNullReference();
            _orchestratorSettings = orchestratorSettingsAccessor.CheckNullReference().CurrentValue;
        }

        public async Task<CreatePurchaseOrderOutput> CreatePurchaseOrderAsync(CreatePurchaseOrderInput input)
        {
            var jobInfo = CreateStartJobInfo(GetProcessKey(nameof(CreatePurchaseOrderAsync)), input);
            var job = await _orchestratorClient.ExecuteJobAsync(jobInfo);
            return JsonConvert.DeserializeObject<CreatePurchaseOrderOutput>(job.OutputArguments);
        }

        public async Task<CreateSalesOrderOutput> CreateSalesOrderAsync(CreateSalesOrderInput input)
        {
            var jobInfo = CreateStartJobInfo(GetProcessKey(nameof(CreateSalesOrderAsync)), input);
            var job = await _orchestratorClient.ExecuteJobAsync(jobInfo);
            return JsonConvert.DeserializeObject<CreateSalesOrderOutput>(job.OutputArguments);
        }

        public async Task<GetItemsOutput> GetItemsAsync(GetItemsInput input)
        {
            var jobInfo = CreateStartJobInfo(GetProcessKey(nameof(GetItemsAsync)), input);
            var job = await _orchestratorClient.ExecuteJobAsync(jobInfo);
            var itemsOutputInternal =  JsonConvert.DeserializeObject<GetItemsOutputInternal>(job.OutputArguments);
            return GetItemsOutputFromInternalResult(itemsOutputInternal);
        }

        public async Task<CancelOrderOutput> CancelOrderAsync(CancelOrderInput input)
        {
            var jobInfo = CreateStartJobInfo(GetProcessKey(nameof(CancelOrderAsync)), input);
            var job = await _orchestratorClient.ExecuteJobAsync(jobInfo);
            return JsonConvert.DeserializeObject<CancelOrderOutput>(job.OutputArguments);
        }
        
        // this is just a simple example. A mapper library is recommended if lots of conversion is needed.
        private GetItemsOutput GetItemsOutputFromInternalResult(GetItemsOutputInternal itemsOutputInternal)
        {
            var items = new List<Item>();
            for (var i = 0; i < itemsOutputInternal.ItemId.Length; i++)
            {
                var item = new Item()
                {
                    DeliveryDate = Convert.ToDateTime(itemsOutputInternal.DeliveryDate[i]),
                    ItemId = itemsOutputInternal.ItemId[i],
                    OrderId = itemsOutputInternal.OrderId[i],
                    ItemName = itemsOutputInternal.ItemName[i],
                    Quantity = itemsOutputInternal.Quantity[i],
                    DeliveryStatus = itemsOutputInternal.DeliveryStatus[i],
                };
                items.Add(item);
            }

            return new GetItemsOutput()
            {
                Items = items.ToArray(),
            };
        }

        private string GetProcessKey(string methodName)
        {
            // default mapping is Process + Async = MethodName. If the procss key not found, it will throw here.
            var processName = methodName.EndsWith("Async") ?
                methodName.Substring(0, methodName.Length - "Async".Length) : methodName;
            return _orchestratorSettings.ProcessKeys.Where(p => p.Process.OrdinalEquals(processName)).First().Key;
        }

        private StartJobInfo CreateStartJobInfo(string processKey)
        {
            return new StartJobInfo()
            {
                ReleaseKey = processKey,
                RobotIds = _orchestratorSettings.RobotIds,
                JobsCount = _orchestratorSettings.JobsCount,
                Strategy = _orchestratorSettings.Strategy,
            };
        }

        private StartJobInfo CreateStartJobInfo<T>(string processKey, T input) where T: class
        {
            return new StartJobInfo()
            {
                ReleaseKey = processKey,
                RobotIds = _orchestratorSettings.RobotIds,
                JobsCount = _orchestratorSettings.JobsCount,
                Strategy = _orchestratorSettings.Strategy,
                InputArguments = JsonConvert.SerializeObject(input),
            };
        }
    }
}
