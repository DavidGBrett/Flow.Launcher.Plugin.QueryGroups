
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class SearchGroupQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.SearchGroup;}

        public bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            bool isEmptyGroupSearch = queryParts.Count == 1 && query.Search != queryParts[0];
            return isEmptyGroupSearch || queryParts.Count == 2;
        }

        public string BuildQuery(string pluginKeyword, string seperator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{seperator}";
        }

        public (string selectedGroup, string itemQuery) ParseQuery(Query query, IReadOnlyList<string> queryParts)
        {
            string selectedGroup = queryParts[0];
            string itemQuery = queryParts.Count > 1 ? queryParts[1] : "";

            return (
                selectedGroup,
                itemQuery
            );
        }
    }
}