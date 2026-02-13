
using System.Collections.Generic;
using System.Diagnostics;
using Flow.Launcher.Plugin.QueryGroups;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class RenameGroupCommandDefinition : ICommandDefinition
    {
        private const string QUERY_KEYWORD = "name";
        public CommandType GetCommandType(){return CommandType.RenameGroup;}


        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[1] == QUERY_KEYWORD;
        }

        public string BuildQuery(string pluginKeyword, string queryGroup, string newGroupName = "")
        {
            var sep = PluginConstants.QuerySeparator;
            return $"{pluginKeyword} {queryGroup}{sep}{QUERY_KEYWORD}{sep}{newGroupName}";
        }

        public (string selectedGroup, string newName) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string newName = queryPartsInfo.Parts.Count > 2 ? queryPartsInfo.Parts[2] : "";

            return (
                selectedGroup,
                newName
            );
        }
    }
}