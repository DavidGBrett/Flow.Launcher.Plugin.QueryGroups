
using System.Collections.Generic;
using System.Diagnostics;
using Flow.Launcher.Plugin.QueryGroups;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddGroupCommandDefinition : ICommandDefinition
    {
        private const string QUERY_KEYWORD = "add";
        public CommandType GetCommandType(){return CommandType.AddGroup;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[0] == QUERY_KEYWORD;
        }

        public string BuildQuery(string pluginKeyword, string newGroupName = "")
        {
            return $"{pluginKeyword} {QUERY_KEYWORD}{PluginConstants.QuerySeparator}{newGroupName}";
        }

        public string ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            return (
                queryPartsInfo.Parts.Count > 1 ? queryPartsInfo.Parts[1] : ""
            );
        }

    }
}