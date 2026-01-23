
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class SearchGroupsQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.SearchGroups;}

        public bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            // one part and no seperator after it
            return queryParts.Count == 1 && query.Search == queryParts[0];
        }

        public string BuildQuery(string pluginKeyword)
        {
            return $"{pluginKeyword} ";
        }

        public string ParseQuery(Query query, IReadOnlyList<string> queryParts)
        {
            string groupQuery = queryParts.Count > 0 ? queryParts[0] : "";

            return (
                groupQuery
            );
        }
    }
}