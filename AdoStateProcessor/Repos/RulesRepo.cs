using AdoStateProcessor.Models;
using AdoStateProcessor.Repos.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace AdoStateProcessor.Repos
{
    public class RulesRepo(IOptions<AdoOptions> adoOptions)
        : IRulesRepo
    {
        private const string RULES_DIR = "rules/";

        public RulesModel LoadProcessTypeRules(string workItemType, string workItemDirectory)
        {
            string srcPathRules = RULES_DIR + adoOptions.Value.ProcessType;

            string ruleFile = Path.Combine(workItemDirectory, srcPathRules, $"rules.{workItemType.ToLower()}.json");

            var converterOptions = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.DeserializeObject<RulesModel>(File.ReadAllText(ruleFile), converterOptions);
        }
    }
}
