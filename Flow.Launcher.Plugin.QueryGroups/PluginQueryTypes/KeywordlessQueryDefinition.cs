
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryTypes
{
    class KeywordlessQueryDefinition : IQueryDefinition
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