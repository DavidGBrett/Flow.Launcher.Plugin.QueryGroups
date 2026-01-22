
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryTypes
{
    interface IQueryType
    {
        static abstract PluginQueryType GetQueryType();
        static abstract bool Matches(Query query, IReadOnlyList<string> queryParts);
    }
}