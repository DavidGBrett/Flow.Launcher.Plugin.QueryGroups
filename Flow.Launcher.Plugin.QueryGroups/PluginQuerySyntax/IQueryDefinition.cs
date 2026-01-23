
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    interface IQueryDefinition
    {
        PluginQueryType GetQueryType();
        bool Matches(QueryPartsInfo queryPartsInfo);
    }
}