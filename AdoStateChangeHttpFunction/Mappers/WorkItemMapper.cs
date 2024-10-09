using AdoStateProcessor.ViewModels;
using Newtonsoft.Json.Linq;
using System;

namespace AdoStateChangeHttpFunction.Mappers
{
    public class WorkItemMapper
    {
        public static WorkItemDto Map(string payload)
        {
            var body = JObject.Parse(payload);
            var workItemDto = new WorkItemDto
            {
                WorkItemId = body["resource"]["workItemId"] != null ? Convert.ToInt32(body["resource"]["workItemId"].ToString()) : -1,
                WorkItemType = body["resource"]["revision"]["fields"]["System.WorkItemType"]?.ToString(),
                EventType = body["eventType"]?.ToString(),
                Rev = body["resource"]["rev"] == null ? -1 : Convert.ToInt32(body["resource"]["rev"].ToString()),
                Url = body["resource"]["url"]?.ToString(),
                TeamProject = body["resource"]["fields"]["System.AreaPath"]?.ToString(),
                State = body["resource"]?["fields"]?["System.State"]?["newValue"]?.ToString()
            };

            return workItemDto;
        }
    }
}
