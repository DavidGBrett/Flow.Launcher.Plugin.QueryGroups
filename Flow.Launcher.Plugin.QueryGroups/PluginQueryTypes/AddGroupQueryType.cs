
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryType
{
    class AddGroupQueryType : IQueryType
    {
        public static bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            return queryParts[0] == "Add";
        }

        public static string BuildQuery(string pluginKeyword, string seperator)
        {
            return $"{pluginKeyword} Add{seperator}";
        }

    }
}