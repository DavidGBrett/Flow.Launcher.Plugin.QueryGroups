
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddItemQueryDefinition : IQueryDefinition
    {
        private const string QUERY_KEYWORD = "Add";
        public PluginQueryType GetQueryType(){return PluginQueryType.AddItem;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[1] == QUERY_KEYWORD;
        }

        public string BuildQuery(string pluginKeyword, string separator, string queryGroup, string newQueryName = "")
        {
            return $"{pluginKeyword} {queryGroup}{separator}{QUERY_KEYWORD}{separator}{newQueryName}";
        }

        public (string selectedGroup, string itemQuery) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string itemQuery = queryPartsInfo.Parts.Count > 2 ? queryPartsInfo.Parts[2] : "";

            return (
                selectedGroup,
                itemQuery
            );
        }

    }
}