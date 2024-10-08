using AdoStateProcessor.Misc;
using AdoStateProcessor.ViewModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoStateChangeHttpFunction.Mappers
{
    public class WorkItemMapper
    {
        public static WorkItemDto Map(string payload)
        {
            var body = JObject.Parse(payload);
            var workItemDto = new WorkItemDto();

            workItemDto.WorkItemId = body["resource"]["workItemId"] != null ? Convert.ToInt32(body["resource"]["workItemId"].ToString()) : -1;
            workItemDto.WorkItemType = body["resource"]["revision"]["fields"]["System.WorkItemType"]?.ToString();
            workItemDto.EventType = body["eventType"]?.ToString();
            workItemDto.Rev = body["resource"]["rev"] == null ? -1 : Convert.ToInt32(body["resource"]["rev"].ToString());
            workItemDto.Url = body["resource"]["url"]?.ToString();
            workItemDto.TeamProject = body["resource"]["fields"]["System.AreaPath"]?.ToString();
            workItemDto.State = body["resource"]["fields"]["System.State"]["newValue"]?.ToString();

            return workItemDto;
        }
    }
}
