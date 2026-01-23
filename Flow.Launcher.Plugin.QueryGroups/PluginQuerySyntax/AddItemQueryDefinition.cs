
using System.Collections.Generic;
using System.Diagnostics;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddItemQueryDefinition : IQueryDefinition
    {
        public PluginQueryType GetQueryType(){return PluginQueryType.AddItem;}
        public bool Matches(Query query, IReadOnlyList<string> queryParts)
        {
            return queryParts[1] == "Add";
        }

        public string BuildQuery(string pluginKeyword, string seperator, string queryGroup)
        {
            return $"{pluginKeyword} {queryGroup}{seperator}Add{seperator}";
        }

        public (string selectedGroup, string itemQuery) ParseQuery(Query query, IReadOnlyList<string> queryParts)
        {
            string selectedGroup = queryParts[0];
            string itemQuery = queryParts.Count > 2 ? queryParts[2] : "";

            return (
                selectedGroup,
                itemQuery
            );
        }

    }
}