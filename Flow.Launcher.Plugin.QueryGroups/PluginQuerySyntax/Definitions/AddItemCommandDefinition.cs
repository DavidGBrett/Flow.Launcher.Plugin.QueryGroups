
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Flow.Launcher.Plugin.QueryGroups;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    class AddItemCommandDefinition : ICommandDefinition
    {
        private const string QUERY_KEYWORD = "add";
        public CommandType GetCommandType(){return CommandType.AddItem;}

        public bool Matches(QueryPartsInfo queryPartsInfo)
        {
            // bool isRightLength = queryPartsInfo.Parts.Count == 3;

            bool hasKeywordInRightPart = queryPartsInfo.Parts[1] == QUERY_KEYWORD;

            // return hasKeywordInRightPart && isRightLength;

            return hasKeywordInRightPart;
        }

        public string BuildQuery(string pluginKeyword, string queryGroup, string newQueryName = "")
        {
            var sep = PluginConstants.QuerySeparator;
            return $"{pluginKeyword} {queryGroup}{sep}{QUERY_KEYWORD}{sep}{newQueryName}";
        }

        public (string selectedGroup, string itemQuery) ParseQuery(QueryPartsInfo queryPartsInfo)
        {
            string selectedGroup = queryPartsInfo.Parts[0];
            // string itemQuery = queryPartsInfo.Parts.Count > 2 ? queryPartsInfo.Parts[2] : "";


            // item query is everything after the second separator
            string itemQuery = "";
            if (queryPartsInfo.Parts.Count > 2)
            {
                itemQuery = string.Join(PluginConstants.QuerySeparator, queryPartsInfo.Parts.Skip(2));
            }

            return (
                selectedGroup,
                itemQuery
            );
        }

    }
}