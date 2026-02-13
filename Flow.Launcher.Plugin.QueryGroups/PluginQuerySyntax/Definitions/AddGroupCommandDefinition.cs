
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            // groupName is everything after the first separator
            // this will catch separators that we dont want, but its important to show an error than silently ignore
            string groupName = "";
            if (queryPartsInfo.Parts.Count > 1)
            {
                groupName = string.Join(PluginConstants.QuerySeparator, queryPartsInfo.Parts.Skip(1));
            }

            return (
                groupName
            );
        }

    }
}