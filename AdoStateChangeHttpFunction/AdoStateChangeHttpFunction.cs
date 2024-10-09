using AdoStateChangeHttpFunction.Mappers;
using AdoStateProcessor.Misc;
using AdoStateProcessor.Processor;
using AdoStateProcessor.Repos.Interfaces;
using AdoStateProcessor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ADOStateChangeHTTPFunction
{
    public class AdoStateChangeHttpFunction(
        IWorkItemRepo workItemRepo,
        IRulesRepo rulesRepo,
        IHelper helper,
        ILogger<AdoStateChangeHttpFunction> logger)
    {
        [Function("AdoStateChangeHttpFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req)
        {
            string requestBody = await req.ReadAsStringAsync();

            string functionAppCurrDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            WorkItemDto workItemRequest = WorkItemMapper.Map(requestBody);

            if (workItemRequest.EventType != "workitem.updated")
            {
                logger.LogCritical("Work item was not updated, yet the trigger somehow triggered. Ignoring request.");
                return new BadRequestObjectResult("Work item was not updated, yet the trigger somehow triggered. Ignoring request.");
            }

            var adoEngine = new AdoProcessor(workItemRepo, rulesRepo, logger);

            await adoEngine.ProcessUpdate(workItemRequest, functionAppCurrDirectory);

            string responseMessage = "This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
