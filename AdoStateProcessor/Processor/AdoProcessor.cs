using AdoStateProcessor.Misc;
using AdoStateProcessor.Models;
using AdoStateProcessor.Repos.Interfaces;
using AdoStateProcessor.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoStateProcessor.Processor
{
    public class AdoProcessor(
        IWorkItemRepo workItemRepo,
        IRulesRepo rulesRepo,
        ILogger logger)
    {
        public async Task ProcessUpdate(WorkItemDto workItemRequest, string functionAppCurrDirectory)
        {
            var relevantRelations = (await workItemRepo.GetWorkItemRelations(workItemRequest.WorkItemId, workItemRequest.WorkItemType))?.ToList();

            if (relevantRelations.Count() == 0)
            {
                logger.LogInformation("No relevant relations found. Exiting.");
                return;
            }

            List<WorkItem> relatedItems = await workItemRepo.GetRelatedItems(relevantRelations);

            if (relatedItems.Count() == 0)
            {
                logger.LogInformation("No related items found. Exiting.");
                return;
            }

            var rulesModel = rulesRepo.LoadProcessTypeRules(workItemRequest.WorkItemType, functionAppCurrDirectory);

            foreach (var rule in rulesModel.Rules)
            {
                logger.LogInformation("Executing against rule:" + rule.IfActorFieldType + rule.AndActorFieldValue);

                if (!Helper.IsMatchingRuleForWorkItem(workItemRequest, rule) || !Helper.IsAffectedWorkItemTypeMatching(relatedItems.First(), rule))
                {
                    logger.LogInformation("Rule not matching. Skipping.");
                    continue;
                }

                // Since we have a hierarchical one to many relation, only tasks can have multiple siblings that need to be checked for completion.
                if (rule.IsAllActors && workItemRequest.WorkItemType == "Task")
                {
                    var actorWorkItems = await workItemRepo.ListChildWorkItemsForParent(relatedItems.First(), rule.IfActorFieldType.ToString());

                    if (Helper.IsAllActorsEqualValues(rule, actorWorkItems))
                    {
                        relatedItems = Helper.FilterExcludedEntries(relatedItems, rule).ToList();
                        await workItemRepo.UpdateWorkItems(relatedItems, ($"System.{rule.WhereAffectedFieldType}", rule.SetAffectedFieldValueTo));
                    }

                    continue;
                }

                logger.LogInformation(" In !rule.AllChildren:" + rule.IfActorFieldType + rule.AndActorFieldValue);

                var includedRelatedItems = Helper.FilterExcludedEntries(relatedItems, rule);

                await workItemRepo.UpdateWorkItems(includedRelatedItems, ($"System.{rule.WhereAffectedFieldType}", rule.SetAffectedFieldValueTo));
            }
        }
        }
    }
}
