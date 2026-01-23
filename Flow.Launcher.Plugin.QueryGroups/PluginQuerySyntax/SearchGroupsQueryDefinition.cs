
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class SearchGroupsQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.SearchGroups;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            // one part and no separator after it
            return queryPartsInfo.Parts.Count == 1 && queryPartsInfo.RawSearchString == queryPartsInfo.Parts[0];
        }

        public string BuildQuery(string pluginKeyword)
        {
            return $"{pluginKeyword} ";
        }

        public string ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string groupQuery = queryPartsInfo.Parts.Count > 0 ? queryPartsInfo.Parts[0] : "";

            return (
                groupQuery
            );
        }
    }
}