
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryType
{
    class KeywordlessQueryType : IQueryType
    {
        public static bool Matches(Query query,IReadOnlyList<string> queryParts)
        {
            return query.ActionKeyword == "";
        }

        public static string BuildQuery()
        {
            return "";
        }

    }
}