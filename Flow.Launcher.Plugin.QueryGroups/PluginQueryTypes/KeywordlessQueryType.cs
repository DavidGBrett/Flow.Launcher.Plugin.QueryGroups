
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryTypes
{
    class KeywordlessQueryTypeHandler : IQueryType
    {
        public static PluginQueryType GetQueryType(){return PluginQueryType.Keywordless;}

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