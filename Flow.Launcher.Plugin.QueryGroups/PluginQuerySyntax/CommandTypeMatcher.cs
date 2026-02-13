using System.Collections.Generic;

namespace Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax
{
    public class CommandTypeMatcher
    {
        private readonly IReadOnlyList<ICommandDefinition> _matchOrder;
        
        public CommandTypeMatcher()
        {
            _matchOrder = new List<ICommandDefinition>
            {
                new KeywordlessCommandDefinition(),
                new SearchGroupsCommandDefinition(),
                new AddGroupCommandDefinition(),
                new AddItemCommandDefinition(),
                new RenameGroupCommandDefinition(),
                new RenameItemCommandDefinition(),
                new ItemQueryAssignmentCommandDefinition(),
                new SearchGroupCommandDefinition(),
            }.AsReadOnly();
        }
        
        public CommandType MatchCommand(QueryPartsInfo queryPartsInfo)
        {
            foreach (var definition in _matchOrder)
            {
                if (definition.Matches(queryPartsInfo))
                {
                    return definition.GetCommandType();
                }
            }

            return CommandType.SearchGroup;
        }
    }
}