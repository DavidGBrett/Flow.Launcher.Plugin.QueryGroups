
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddGroupQueryDefinition : IQueryDefinition
    {
        private const string QUERY_KEYWORD = "add";
        public PluginQueryType GetQueryType(){return PluginQueryType.AddGroup;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[0] == QUERY_KEYWORD;
        }

        public string BuildQuery(string pluginKeyword, string separator, string newGroupName = "")
        {
            return $"{pluginKeyword} {QUERY_KEYWORD}{separator}{newGroupName}";
        }

        public string ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            return (
                queryPartsInfo.Parts.Count > 1 ? queryPartsInfo.Parts[1] : ""
            );
        }

    }
}