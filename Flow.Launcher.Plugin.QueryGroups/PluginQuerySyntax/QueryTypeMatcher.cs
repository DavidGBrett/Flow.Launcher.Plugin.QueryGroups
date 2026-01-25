using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public class QueryTypeMatcher
    {
        private readonly IReadOnlyList<IQueryDefinition> _matchOrder;
        
        public QueryTypeMatcher()
        {
            _matchOrder = new List<IQueryDefinition>
            {
                new KeywordlessQueryDefinition(),
                new SearchGroupsQueryDefinition(),
                new AddGroupQueryDefinition(),
                new AddItemQueryDefinition(),
                new AddItemNameQueryDefinition(),
                new RenameGroupQueryDefinition(),
                new RenameItemDefinition(),
                new ItemQueryAssignmentQueryDefinition(),
                new SearchGroupQueryDefinition(),
            }.AsReadOnly();
        }
        
        public PluginQueryType MatchQueryType(QueryPartsInfo queryPartsInfo)
        {
            foreach (var definition in _matchOrder)
            {
                if (definition.Matches(queryPartsInfo))
                {
                    return definition.GetQueryType();
                }
            }

            return PluginQueryType.SearchGroup;
        }
    }
}