
using System.Collections.Generic;
using System.Diagnostics;
using Flow.Launcher.Plugin.QueryGroups;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class RenameItemCommandDefinition : ICommandDefinition
    {
        private const string QUERY_KEYWORD = "name";
        public CommandType GetCommandType(){return CommandType.RenameItem;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts.Count >= 3 && queryPartsInfo.Parts[2] == QUERY_KEYWORD;
        }

        public string BuildQuery(string pluginKeyword, string queryGroup, string queryItem, string newItemName = "")
        {
            var sep = PluginConstants.QuerySeparator;
            return $"{pluginKeyword} {queryGroup}{sep}{queryItem}{sep}{QUERY_KEYWORD}{sep}{newItemName}";
        }

        public (string selectedGroup, string selectedItem, string newItemName) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string selectedItem = queryPartsInfo.Parts[1];
            string newItemName = queryPartsInfo.Parts.Count > 3 ? queryPartsInfo.Parts[3] : "";

            return (
                selectedGroup,
                selectedItem,
                newItemName
            );
        }
    }
}