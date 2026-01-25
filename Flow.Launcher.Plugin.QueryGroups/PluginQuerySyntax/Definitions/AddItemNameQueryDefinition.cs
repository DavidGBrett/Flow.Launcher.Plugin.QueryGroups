
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddItemNameQueryDefinition : IQueryDefinition
    {
        private const string QUERY_KEYWORD = "add";
        public PluginQueryType GetQueryType(){return PluginQueryType.AddItemName;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            bool isRightLength = queryPartsInfo.Parts.Count == 4;
            
            bool hasKeywordInRightPart = queryPartsInfo.Parts[1] == QUERY_KEYWORD;

            return hasKeywordInRightPart && isRightLength;
        }

        public string BuildQuery(string pluginKeyword, string separator, string queryGroup, string newItemQuery = "", string newItemName = "")
        {
            string sep = separator;
            return $"{pluginKeyword} {queryGroup}{sep}{QUERY_KEYWORD}{sep}{newItemQuery}{sep}{newItemName}";
        }

        public (string selectedGroup, string itemQuery, string itemName) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            string itemQuery = queryPartsInfo.Parts.Count > 2 ? queryPartsInfo.Parts[2] : "";
            string itemName = queryPartsInfo.Parts.Count > 3 ? queryPartsInfo.Parts[3] : "";

            return (
                selectedGroup,
                itemQuery,
                itemName
            );
        }

    }
}