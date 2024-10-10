using AdoStateProcessor.Factories;
using AdoStateProcessor.Misc;
using AdoStateProcessor.Repos.Interfaces;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdoStateProcessor.Repos
{
    public class WorkItemRepo(IAdoFactory adoFactory, IHelper helper) : IWorkItemRepo
    {
        private readonly IVssConnection _connection = adoFactory.Create();

        public async Task<WorkItem> GetWorkItem(int id)
        {
            var client = _connection.GetClient<WorkItemTrackingHttpClient>();
            try
            {
                return await client.GetWorkItemAsync(id, null, null, WorkItemExpand.Relations);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception e:" + e.GetBaseException());
                return null;
            }
        }

        public async Task<List<WorkItem>> ListChildWorkItemsForParent(WorkItem parentWorkItem, string fieldName)
        {
            var client = _connection.GetClient<WorkItemTrackingHttpClient>();

            var childrenIds = parentWorkItem.Relations.Where(x => x.Rel.Equals("System.LinkTypes.Hierarchy-Forward"))
                                            .Select(x => helper.GetWorkItemIdFromUrl(x.Url)).ToList();

            string[] fields = [$"System.{fieldName}"];

            return await client.GetWorkItemsAsync(childrenIds, fields);
        }

        public async Task<WorkItem> UpdateWorkItem(WorkItem workItem, (string fieldName, string value) fieldSet)
        {
            JsonPatchDocument patchDocument =
            [
                new JsonPatchOperation()
                {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = workItem.Rev.ToString()
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = $"/fields/{fieldSet.fieldName}",
                    Value = fieldSet.value
                }
            ];

            try
            {
                var client = _connection.GetClient<WorkItemTrackingHttpClient>();
                return await client.UpdateWorkItemAsync(patchDocument, workItem.Id.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<WorkItem>> UpdateWorkItems(IEnumerable<WorkItem> workItems, (string fieldName, string value) fieldSet)
        {
            return await Task.WhenAll(workItems.Select(item => UpdateWorkItem(item, fieldSet)));
        }

        public async Task<IEnumerable<WorkItemRelation>> GetWorkItemRelations(int workItemId, string workItemType)
        {
            var workItem = await GetWorkItem(workItemId) ?? throw new ApplicationException("Failed to get work item.");

            // For simplicity we are assuming that we're either dealing with Tasks or PBI's/Bugs
            var hierarchyDirection = workItemType == "Task" ? "System.LinkTypes.Hierarchy-Reverse" : "System.LinkTypes.Hierarchy-Forward";

            return workItem.Relations.Where(x => x.Rel.Equals(hierarchyDirection));
        }

        public async Task<List<WorkItem>> GetRelatedItems(List<WorkItemRelation> relevantRelations)
        {
            var relatedUris = relevantRelations.Select(x => helper.GetWorkItemIdFromUrl(x.Url));

            var relatedItems = (await Task.WhenAll(relatedUris.Select(GetWorkItem)))?.Where(result => result != null).ToList();
            return relatedItems;
        }
    }
}
