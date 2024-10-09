namespace AdoStateProcessor.ViewModels
{
    public class WorkItemDto
    {
        public int WorkItemId { get; set; }
        public string WorkItemType { get; set; }
        public int ParentId { get; set; }
        public int ParentUrl { get; set; }
        public string EventType { get; set; }
        public int Rev { get; set; }
        public string TeamProject { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
        public string IterationPath { get; set; }
    }
}
