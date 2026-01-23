
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddGroupQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.AddGroup;}
        public bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            return queryParts[0] == "Add";
        }

        public string BuildQuery(string pluginKeyword, string seperator)
        {
            return $"{pluginKeyword} Add{seperator}";
        }

    }
}