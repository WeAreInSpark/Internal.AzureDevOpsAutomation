using AdoStateProcessor.Models;

namespace AdoStateProcessor.Repos.Interfaces
{
    public interface IRulesRepo
    {
        public RulesModel LoadProcessTypeRules(string workItemType, string workItemDirectory);
    }
}
