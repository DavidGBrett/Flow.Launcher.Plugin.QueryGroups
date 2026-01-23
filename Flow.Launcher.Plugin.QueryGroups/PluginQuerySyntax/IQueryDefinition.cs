
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    interface IQueryDefinition
    {
        PluginQueryType GetQueryType();
        bool Matches(Query query, IReadOnlyList<string> queryParts);
    }
}