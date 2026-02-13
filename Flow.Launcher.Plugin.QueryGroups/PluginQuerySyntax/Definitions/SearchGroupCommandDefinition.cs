
using System.Collections.Generic;
using System.Diagnostics;
using Flow.Launcher.Plugin.QueryGroups;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class SearchGroupCommandDefinition : ICommandDefinition
    {
        public CommandType GetCommandType(){return CommandType.SearchGroup;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            bool isEmptyGroupSearch = queryPartsInfo.Parts.Count == 1 && queryPartsInfo.RawSearchString != queryPartsInfo.Parts[0];
            return isEmptyGroupSearch || queryPartsInfo.Parts.Count == 2;
        }

        public string BuildQuery(string pluginKeyword, string queryGroup, string querySearch = "")
        {
            var sep = PluginConstants.QuerySeparator;
            return $"{pluginKeyword} {queryGroup}{sep}{querySearch}";
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