
using System;
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public class QueryPartsInfo
    {
        public readonly string RawSearchString;
        public readonly string ActionKeyword;
        public readonly IReadOnlyList<string> Parts;
        public readonly bool EndsWithSeparator;

        public QueryPartsInfo(
            string rawSearchString,
            string actionKeyword
        ){
            this.RawSearchString = rawSearchString ?? "";
            this.ActionKeyword = actionKeyword ?? "";

            Parts = rawSearchString.Split(new string[] { PluginConstants.QuerySeparator }, StringSplitOptions.None);;
            EndsWithSeparator = rawSearchString.EndsWith(PluginConstants.QuerySeparator);
        }
    };
}