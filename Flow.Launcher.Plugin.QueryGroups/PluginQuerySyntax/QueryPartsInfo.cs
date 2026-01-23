
using System;
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public class QueryPartsInfo
    {
        public readonly string RawSearchString;
        public readonly string ActionKeyword;
        public readonly string Separator;
        public readonly IReadOnlyList<string> Parts;
        public readonly bool EndsWithSeparator;

        public QueryPartsInfo(
            string rawSearchString,
            string actionKeyword,
            string separator
        ){
            if (string.IsNullOrEmpty(separator))
                throw new ArgumentException("Separator cannot be empty or null");

            this.Separator = separator;
            this.RawSearchString = rawSearchString ?? "";
            this.ActionKeyword = actionKeyword ?? "";
            

            Parts = rawSearchString.Split(new string[] { separator }, StringSplitOptions.None);;
            EndsWithSeparator = rawSearchString.EndsWith(separator);
        }
    };
}