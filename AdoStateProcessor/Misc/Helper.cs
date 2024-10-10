using AdoStateProcessor.Models;
using AdoStateProcessor.ViewModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdoStateProcessor.Misc
{
    public class Helper
    {
        public static int GetWorkItemIdFromUrl(string url)
        {
            int lastIndexOf = url.LastIndexOf("/");
            int size = url.Length - (lastIndexOf + 1);

            string value = url.Substring(lastIndexOf + 1, size);

            if (!int.TryParse(value, out var workItemId))
                throw new ArgumentException("Invalid work item id");

            return workItemId;
        }
        public static bool IsAffectedWorkItemTypeMatching(WorkItem affectedItem, Rule rule)
        {
            return rule.AffectedType == affectedItem.Fields["System.WorkItemType"].ToString();
        }

        public static IEnumerable<WorkItem> FilterExcludedEntries(List<WorkItem> relatedItems, Rule rule)
        {
            return relatedItems.Where(x => !rule.AndNotAffectedFieldValues.Contains(x.Fields[$"System.{rule.WhereAffectedFieldType}"]?.ToString()));
        }

        public static bool IsMatchingRuleForWorkItem(WorkItemDto workItemRequest, Rule rule)
        {
            return workItemRequest.GetType().GetProperties().Any(x => x.Name == rule.IfActorFieldType.ToString() && x.GetValue(workItemRequest)?.ToString() == rule.AndActorFieldValue.ToString());
        }
        public static bool IsAllActorsEqualValues(Rule rule, List<WorkItem> actorWorkItems)
        {
            return actorWorkItems.All(x => x.Fields[$"System.{rule.IfActorFieldType}"].ToString() == rule.AndActorFieldValue);
        }
    }
}
