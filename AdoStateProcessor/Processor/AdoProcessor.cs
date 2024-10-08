using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AdoStateProcessor.Repos.Interfaces;
using AdoStateProcessor.Misc;
using AdoStateProcessor.ViewModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.Extensions.Options;

namespace AdoStateProcessor.Processor
{
    public class AdoProcessor(
        IWorkItemRepo workItemRepo,
        IRulesRepo rulesRepo,
        IHelper helper,
        ILogger logger)
    {
        public async Task ProcessUpdate(WorkItemDto workItemRequest, string functionAppCurrDirectory)
        {
            WorkItemRelation parentRelation = await workItemRepo.GetWorkItemParent(workItemRequest.WorkItemId);

            var parentId = helper.GetWorkItemIdFromUrl(parentRelation.Url);
            var parentWorkItem = await workItemRepo.GetWorkItem(parentId) ?? throw new ApplicationException("Failed to get parent work item.");
            var parentState = parentWorkItem.Fields["System.State"]?.ToString() ?? string.Empty;

            var rulesModel = rulesRepo.LoadProcessTypeRules(workItemRequest.WorkItemType, functionAppCurrDirectory);

            foreach (var rule in rulesModel.Rules)
            {
                logger.LogInformation(" Executing against rule:" + rule.IfChildState);

                if (!rule.IfChildState.Equals(workItemRequest.State))
                {
                    continue;
                }

                if (rule.AllChildren)
                {
                    var childWorkItems = await workItemRepo.ListChildWorkItemsForParent(parentWorkItem);

                    var isAllChildrenEqualStates = childWorkItems.All(x => x.Fields["System.State"].ToString() == rule.IfChildState);

                    if (isAllChildrenEqualStates)
                        await workItemRepo.UpdateWorkItemState(parentWorkItem, rule.SetParentStateTo);
                }
                else
                {
                    logger.LogInformation(" In !rule.AllChildren:" + workItemRequest.State);
                    if (!rule.NotParentStates.Contains(parentState))
                    {
                        await workItemRepo.UpdateWorkItemState(parentWorkItem, rule.SetParentStateTo);
                    }
                }
            }
        }
    }
}
