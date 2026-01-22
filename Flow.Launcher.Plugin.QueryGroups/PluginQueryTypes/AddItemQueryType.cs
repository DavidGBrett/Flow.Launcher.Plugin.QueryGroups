
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryType
{
    class AddItemQueryType : IQueryType
    {
        public static bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            return queryParts[1] == "Add";
        }

        public static string BuildQuery(string pluginKeyword, string seperator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{seperator}Add{seperator}";
        }

    }
}