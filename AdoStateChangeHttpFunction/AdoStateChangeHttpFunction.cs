using AdoStateProcessor.Misc;
using AdoStateProcessor.Processor;
using AdoStateProcessor.Repos.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ADOStateChangeHTTPFunction
{
    public class AdoStateChangeHttpFunction
    {
        private readonly IWorkItemRepo _workItemRepo;
        private readonly IRulesRepo _rulesRepo;
        private readonly IHelper _helper;
        private readonly ILogger<AdoStateChangeHttpFunction> _logger;

        public AdoStateChangeHttpFunction(
            IWorkItemRepo workItemRepo,
            IRulesRepo rulesRepo,
            IHelper helper,
            ILogger<AdoStateChangeHttpFunction> logger)
        {
            _workItemRepo = workItemRepo;
            _rulesRepo = rulesRepo;
            _helper = helper;
            _logger = logger;
        }

        [Function("AdoStateChangeHttpFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(" C# HTTP trigger function processed a request.");
            
            //make sure pat is not empty
            string pat = Environment.GetEnvironmentVariable("ADO_PAT");
            if (string.IsNullOrEmpty(pat))
            {
                _logger.LogCritical(" Pat not found to process, exiting");
                return new BadRequestObjectResult("Pat not found to process, exiting");
            }
            
            //make sure processType is not empty, otherwise default to scrum
            string processType = Environment.GetEnvironmentVariable("ADO_PROCESS_TYPE") ?? "scrum";
            _logger.LogInformation(" ProcessType:" + processType);
            
            //Parse request body as JObject
            JObject payload = JObject.Parse(requestBody);

            // Need to read the rules file from the rules folder in current context
            string functionAppCurrDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var adoEngine = new AdoProcessor(_workItemRepo, _rulesRepo, _helper, _logger);

            Task.WaitAll(
                adoEngine.ProcessUpdate(payload, pat, functionAppCurrDirectory, processType));

            string responseMessage = "This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

    }



}
