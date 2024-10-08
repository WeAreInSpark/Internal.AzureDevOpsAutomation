using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

using AdoStateProcessor.Misc;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using System.Threading.Tasks;
using AdoStateProcessor.Repos.Interfaces;
using AdoStateProcessor.Factories;
using AdoStateProcessor.ViewModels;

namespace AdoStateProcessor.Repos
{
    public class WorkItemRepo(IAdoFactory adoFactory, IHelper helper) : IWorkItemRepo
    {
        private readonly IHelper _helper = helper;
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

        public async Task<List<WorkItem>> ListChildWorkItemsForParent(WorkItem parentWorkItem)
        {
            var client = _connection.GetClient<WorkItemTrackingHttpClient>();

            var childrenIds = parentWorkItem.Relations.Where(x => x.Rel.Equals("System.LinkTypes.Hierarchy-Forward"))
                                            .Select(x => _helper.GetWorkItemIdFromUrl(x.Url)).ToList();

            string[] fields = new string[] { "System.State" };

            return await client.GetWorkItemsAsync(childrenIds, fields);
        }

        public async Task<WorkItem> UpdateWorkItemState(WorkItem workItem, string state)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = workItem.Rev.ToString()
                },
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.State",
                    Value = state
                }
            };

            try
            {
                var client = _connection.GetClient<WorkItemTrackingHttpClient>();
                return await client.UpdateWorkItemAsync(patchDocument, Convert.ToInt32(workItem.Id));
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<WorkItemRelation> GetWorkItemParent(int workItemId)
        {
            var workItem = await GetWorkItem(workItemId) ?? throw new ApplicationException("Failed to get work item.");

            return workItem.Relations.Where(x => x.Rel.Equals("System.LinkTypes.Hierarchy-Reverse")).FirstOrDefault() ?? throw new ApplicationException("Failed to get related items.");
        }
    }
}
