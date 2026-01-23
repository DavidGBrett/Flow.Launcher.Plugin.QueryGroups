
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class RenameGroupQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.RenameGroup;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            return queryPartsInfo.Parts[1] == "Rename";
        }

        public string BuildQuery(string pluginKeyword, string separator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{separator}Rename{separator}";
        }

        public (string selectedGroup, string newName) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string newName = queryPartsInfo.Parts.Count > 2 ? queryPartsInfo.Parts[2] : "";

            return (
                selectedGroup,
                newName
            );
        }
    }
}