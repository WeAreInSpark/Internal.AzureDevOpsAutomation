using System;

namespace AdoStateProcessor.Misc
{
    public class Helper : IHelper
    {
        public int GetWorkItemIdFromUrl(string url)
        {
            int lastIndexOf = url.LastIndexOf("/");
            int size = url.Length - (lastIndexOf + 1);

            string value = url.Substring(lastIndexOf + 1, size);

            if (!int.TryParse(value, out var workItemId))
                throw new ArgumentException("Invalid work item id");

            return workItemId;
        }
    }
}
