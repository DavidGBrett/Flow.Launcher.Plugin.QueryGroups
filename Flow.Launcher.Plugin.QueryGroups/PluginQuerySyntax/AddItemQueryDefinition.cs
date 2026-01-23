
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddItemQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.AddItem;}
        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[1] == "Add";
        }

        public string BuildQuery(string pluginKeyword, string seperator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{seperator}Add{seperator}";
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