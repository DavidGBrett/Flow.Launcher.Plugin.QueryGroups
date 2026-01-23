
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public record QueryPartsInfo
    (
        string RawSearchString,
        string ActionKeyword,
        string Seperator,
        IReadOnlyList<string> Parts,
        bool EndsWithSeparator
    );
}