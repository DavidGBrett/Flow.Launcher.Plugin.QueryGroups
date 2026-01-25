
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class ItemQueryAssignmentCommandDefinition : ICommandDefinition
    {
        private const string QUERY_KEYWORD = "set";
        public CommandType GetCommandType(){return CommandType.SetItemQuery;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts.Count >= 3 && queryPartsInfo.Parts[2] == QUERY_KEYWORD;
        }

        public string BuildQuery(string pluginKeyword, string separator, string queryGroup, string queryItem, string newItemQuery = "")
        {
            var sep = separator;
            return $"{pluginKeyword} {queryGroup}{sep}{queryItem}{sep}{QUERY_KEYWORD}{sep}{newItemQuery}";
        }

        public (string selectedGroup, string selectedItem, string newItemQuery) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string selectedItem = queryPartsInfo.Parts[1];
            string newItemQuery = queryPartsInfo.Parts.Count > 3 ? queryPartsInfo.Parts[3] : "";

            return (
                selectedGroup,
                selectedItem,
                newItemQuery
            );
        }
    }
}