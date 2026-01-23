
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class SearchGroupsQueryDefinition : IQueryDefinition
    {
        public static PluginQueryType GetQueryType(){return PluginQueryType.SearchGroups;}

        public static bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            // one part and no seperator after it
            return queryParts.Count == 1 && query.Search == queryParts[0];
        }

        public static string BuildQuery(string pluginKeyword)
        {
            return $"{pluginKeyword} ";
        }

    }
}