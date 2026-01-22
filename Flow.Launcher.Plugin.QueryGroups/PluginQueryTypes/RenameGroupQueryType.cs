
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryType
{
    class RenameGroupQueryType : IQueryType
    {
        public static bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            return queryParts[1] == "Rename";
        }

        public static string BuildQuery(string pluginKeyword, string seperator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{seperator}Rename{seperator}";
        }

    }
}