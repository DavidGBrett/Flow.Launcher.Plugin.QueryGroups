
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public interface IQueryDefinition
    {
        PluginQueryType GetQueryType();
        bool Matches(QueryPartsInfo queryPartsInfo);
    }
}