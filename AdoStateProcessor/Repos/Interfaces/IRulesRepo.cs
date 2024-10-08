using AdoStateProcessor.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdoStateProcessor.Repos.Interfaces
{
    public interface IRulesRepo
    {
        public RulesModel LoadProcessTypeRules(string workItemType, string workItemDirectory);
    }
}
