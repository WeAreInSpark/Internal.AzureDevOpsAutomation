using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdoStateProcessor.Repos.Interfaces
{
    public interface IWorkItemRepo
    {
        Task<WorkItem> GetWorkItem(int id);
        Task<List<WorkItem>> ListChildWorkItemsForParent(WorkItem parentWorkItem, string fieldName);
        Task<WorkItem> UpdateWorkItem(WorkItem workItem, (string fieldName, string value) fieldSet);
        IEnumerable<WorkItemRelation> GetWorkItemRelations(WorkItem workItem, string workItemType);
        Task<IEnumerable<WorkItem>> UpdateWorkItems(IEnumerable<WorkItem> workItems, (string fieldName, string value) fieldSet);
        Task<List<WorkItem>> GetRelatedItems(List<WorkItemRelation> relevantRelations);
    }
}
