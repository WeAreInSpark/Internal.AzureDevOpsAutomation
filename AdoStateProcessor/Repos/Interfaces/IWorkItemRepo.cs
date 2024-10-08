using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdoStateProcessor.Repos.Interfaces
{
    public interface IWorkItemRepo
    {
        Task<WorkItem> GetWorkItem(int id);
        Task<List<WorkItem>> ListChildWorkItemsForParent(WorkItem parentWorkItem);
        Task<WorkItem> UpdateWorkItemState(WorkItem workItem, string state);
        Task<WorkItemRelation> GetWorkItemParent(int workItemId);
    }
}
