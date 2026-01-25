
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class KeywordlessCommandDefinition : ICommandDefinition
    {
        public CommandType GetCommandType(){return CommandType.Keywordless;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.ActionKeyword == "";
        }

        public string BuildQuery(string rawSearchString = "")
        {
            return rawSearchString;
        }

        public string ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            return (
                queryPartsInfo.RawSearchString
            );
        }

    }
}