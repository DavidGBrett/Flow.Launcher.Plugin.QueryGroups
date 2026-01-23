
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class KeywordlessQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.Keywordless;}

        public bool Matches(Query query,IReadOnlyList<string> queryParts)
        {
            return query.ActionKeyword == "";
        }

        public string BuildQuery()
        {
            return "";
        }

    }
}