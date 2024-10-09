namespace AdoStateProcessor.Models
{
    public class RulesModel
    {
        public string ActorType { get; set; }

        public Rule[] Rules { get; set; }
    }

    public class Rule
    {
        public string AffectedType { get; set; }
        public WorkItemProperty IfActorFieldType { get; set; }

        public string AndActorFieldValue { get; set; }

        public WorkItemProperty WhereAffectedFieldType { get; set; }
        public string[] AndNotAffectedFieldValues { get; set; }

        public string SetAffectedFieldValueTo { get; set; }

        public bool IsAllActors { get; set; } = false;
    }
    public enum WorkItemProperty
    {
        State,
        IterationPath
    }
}
