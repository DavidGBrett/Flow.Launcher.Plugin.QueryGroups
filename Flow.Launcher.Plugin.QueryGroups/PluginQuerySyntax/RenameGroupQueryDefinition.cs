
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class RenameGroupQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.RenameGroup;}

        public bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            return queryParts[1] == "Rename";
        }

        public string BuildQuery(string pluginKeyword, string seperator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{seperator}Rename{seperator}";
        }

        public (string selectedGroup, string newName) ParseQuery(Query query, IReadOnlyList<string> queryParts)
        {
            string selectedGroup = queryParts[0];
            string newName = queryParts.Count > 2 ? queryParts[2] : "";

            return (
                selectedGroup,
                newName
            );
        }
    }
}