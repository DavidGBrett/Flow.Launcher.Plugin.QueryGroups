
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddGroupQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.AddGroup;}
        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[0] == "Add";
        }

        public string BuildQuery(string pluginKeyword, string separator)
        {
            return $"{pluginKeyword} Add{separator}";
        }

        public string ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            return (
                queryPartsInfo.Parts.Count > 1 ? queryPartsInfo.Parts[1] : ""
            );
        }

    }
}