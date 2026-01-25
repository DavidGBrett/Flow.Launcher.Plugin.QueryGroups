
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public interface ICommandDefinition
    {
        CommandType GetCommandType();
        bool Matches(QueryPartsInfo queryPartsInfo);
    }
}