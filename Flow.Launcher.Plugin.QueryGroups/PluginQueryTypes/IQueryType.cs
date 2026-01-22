
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQueryType
{
    interface IQueryType
    {
        static abstract bool Matches(Query query, IReadOnlyList<string> queryParts);
    }
}