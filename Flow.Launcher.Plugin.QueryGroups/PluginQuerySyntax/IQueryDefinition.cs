
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    interface IQueryDefinition
    {
        static abstract PluginQueryType GetQueryType();
        static abstract bool Matches(Query query, IReadOnlyList<string> queryParts);
    }
}