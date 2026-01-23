
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class SearchGroupQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.SearchGroup;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            bool isEmptyGroupSearch = queryPartsInfo.Parts.Count == 1 && queryPartsInfo.RawSearchString != queryPartsInfo.Parts[0];
            return isEmptyGroupSearch || queryPartsInfo.Parts.Count == 2;
        }

        public string BuildQuery(string pluginKeyword, string separator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{separator}";
        }

        public (string selectedGroup, string itemQuery) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string itemQuery = queryPartsInfo.Parts.Count > 1 ? queryPartsInfo.Parts[1] : "";

            return (
                selectedGroup,
                itemQuery
            );
        }
    }
}