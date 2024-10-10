using AdoStateProcessor.Misc;
using AdoStateProcessor.Models;
using AdoStateProcessor.ViewModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Internal.AzureDevOpsAutomation.UnitTests
{
    public class HelperTests
    {
        [Theory]
        [InlineData("https://dev.azure.com/organization/project/_workitems/edit/123", 123)]
        [InlineData("https://dev.azure.com/organization/project/_workitems/edit/456", 456)]
        public void GetWorkItemIdFromUrl_ValidUrl_ReturnsWorkItemId(string url, int expectedWorkItemId)
        {
            // Act
            int workItemId = Helper.GetWorkItemIdFromUrl(url);

            // Assert
            Assert.Equal(expectedWorkItemId, workItemId);
        }

        [Theory]
        [InlineData("https://dev.azure.com/organization/project/_workitems/edit/abc")]
        [InlineData("https://dev.azure.com/organization/project/_workitems/edit/xyz")]
        public void GetWorkItemIdFromUrl_InvalidUrl_ThrowsArgumentException(string url)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Helper.GetWorkItemIdFromUrl(url));
        }

        [Theory]
        [InlineData("Bug", "Bug", true)]
        [InlineData("Product Backlog Item", "Bug", false)]
        [InlineData("Product Backlog Item", "Task", false)]
        [InlineData("Product Backlog Item", "Product Backlog Item", true)]
        public void IsAffectedWorkItemTypeMatching_AffectedItemTypeMatches_ReturnsTrue(string affectedItemType, string ruleAffectedType, bool expectedResult)
        {
            // Arrange
            var affectedItem = new WorkItem();
            affectedItem.Fields["System.WorkItemType"] = affectedItemType;
            var rule = new Rule { AffectedType = ruleAffectedType };

            // Act
            bool result = Helper.IsAffectedWorkItemTypeMatching(affectedItem, rule);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("Value1", new string[] { "Value2", "Value3" }, 1)]
        [InlineData("Value1", new string[] { "Value1", "Value5" }, 0)]
        public void FilterExcludedEntries_ExcludedFieldValuesPresent_ReturnsFilteredItems(string value1, string[] excludedValues, int expected)
        {
            // Arrange
            var relatedItems = new List<WorkItem>
                {
                    new WorkItem { Fields = new Dictionary<string, object> { { "System.State", value1 } } }
                };
            var rule = new Rule { WhereAffectedFieldType = WorkItemProperty.State, AndNotAffectedFieldValues = excludedValues };

            // Act
            IEnumerable<WorkItem> filteredItems = Helper.FilterExcludedEntries(relatedItems, rule);

            // Assert
            Assert.Equal(filteredItems.Count(), expected);
        }

        [Theory]
        [InlineData("Active", "", WorkItemProperty.State, "Active", true)]
        [InlineData("", "Foo", WorkItemProperty.IterationPath, "Foo", true)]
        [InlineData("Active", "", WorkItemProperty.State, "Bla", false)]
        [InlineData("", "Foo", WorkItemProperty.IterationPath, "Kek", false)]
        public void IsMatchingRuleForWorkItem_MatchingRule_ReturnsCorrectValue(string state, string iterationPath, WorkItemProperty actorFieldType, string actorFieldValue, bool expected)
        {
            // Arrange
            var workItemRequest = new WorkItemDto { WorkItemId = 1, WorkItemType = "Bug", State = state, IterationPath = iterationPath };
            var rule = new Rule { IfActorFieldType = actorFieldType, AndActorFieldValue = actorFieldValue };

            // Act
            bool result = Helper.IsMatchingRuleForWorkItem(workItemRequest, rule);

            // Assert
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(WorkItemProperty.State, new string[] { "Active", "Active" }, true)]
        [InlineData(WorkItemProperty.IterationPath, new string[] { "Active", "Inactive" }, false)]
        public void IsAllActorsEqualValues_ShouldReturnCorrectResult(WorkItemProperty actorFieldType, string[] actorFieldValue, bool expectedResult)
        {
            // Arrange
            var rule = new Rule
            {
                IfActorFieldType = actorFieldType,
                AndActorFieldValue = actorFieldValue[0]
            };

            var actorWorkItems = new List<WorkItem>
            {
                new WorkItem { Fields = new Dictionary<string, object> { { $"System.{actorFieldType}", actorFieldValue[0] } } },
                new WorkItem { Fields = new Dictionary<string, object> { { $"System.{actorFieldType}", actorFieldValue[1] } } }
            };

            // Act
            bool result = Helper.IsAllActorsEqualValues(rule, actorWorkItems);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
