using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Flow.Launcher.Plugin;
using Flow.Launcher.Plugin.QueryGroups.PluginQuerySyntax;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroups : IPlugin, ISettingProvider, IContextMenu
    {
        private PluginInitContext _context;

        private Settings _settings;
        private SettingsViewModel _viewModel;

        private string mainPluginKeyword;
        public void Init(PluginInitContext context)
        {
            _context = context;

            _settings = _context.API.LoadSettingJsonStorage<Settings>();
            _viewModel = new SettingsViewModel(_settings);

            mainPluginKeyword = _context.CurrentPluginMetadata.ActionKeywords[1];
        }

        public Control CreateSettingPanel()
        {
            return new SettingsControl(_context, _viewModel);
        }

        public List<Result> Query(Query query)
        {
            // Split the query into parts based on the separator
            var queryPartsInfo = new QueryPartsInfo(
                rawSearchString: query.Search,
                actionKeyword: query.ActionKeyword
            );
            
            // Determine the type of command
            var commandTypeMatcher = new CommandTypeMatcher();
            var queryCommandType = commandTypeMatcher.MatchCommand(queryPartsInfo);

            // Handle the query command based on its type
            switch (queryCommandType)
            {
                case CommandType.Keywordless:
                    {
                        string search = new KeywordlessCommandDefinition().ParseQuery(queryPartsInfo);
                        List<Result> results = GetGroupsResults(search);
                        return results;
                    }

                case CommandType.SearchGroups:
                    {
                        string groupQuery = new SearchGroupsCommandDefinition().ParseQuery(queryPartsInfo);

                        List<Result> results = GetGroupsResults(groupQuery);
                        results.Add(GetAddGroupResult());

                        return results;
                    }

                case CommandType.AddGroup:
                    {
                        string newGroupName = new AddGroupCommandDefinition().ParseQuery(queryPartsInfo);
                        return GetAddGroupResults(newGroupName);
                    }

                case CommandType.SearchGroup:
                    {
                        (string selectedGroup, string itemQuery) = new SearchGroupCommandDefinition().ParseQuery(queryPartsInfo);

                        List<Result> results = GetGroupItemsResults(selectedGroup, itemQuery);
                        results.Add(GetAddItemResult(selectedGroup));

                        return results;
                    }

                case CommandType.AddItem:
                    {
                        (string selectedGroup, string itemQuery) = new AddItemCommandDefinition().ParseQuery(queryPartsInfo);

                        return GetAddItemResults(selectedGroup, itemQuery);
                    }

                case CommandType.RenameGroup:
                    {
                        (string selectedGroup, string newName) = new RenameGroupCommandDefinition().ParseQuery(queryPartsInfo);

                        return GetRenameGroupResults(selectedGroup,newName);
                    }

                case CommandType.RenameItem:
                    {
                        (string selectedGroup, string selectedItem, string newItemName) = new RenameItemCommandDefinition().ParseQuery(queryPartsInfo);

                        return GetRenameItemResults(selectedGroup, selectedItem, newItemName);
                    }
                
                case CommandType.SetItemQuery:
                    {
                        (string selectedGroup, string selectedItem, string newItemQuery) = new ItemQueryAssignmentCommandDefinition().ParseQuery(queryPartsInfo);

                        return GetSetItemQueryResults(selectedGroup, selectedItem, newItemQuery);
                    }

                default:
                    return new List<Result>();
            }
        }

        private List<Result> GetGroupItemsResults(string selectedGroup, string itemQuery)
        {
            List<Result> results = new List<Result>();

            

            foreach (var group in _settings.QueryGroups)
            {
                // continue if this group doesnt match
                if (group.Name != selectedGroup)
                { continue; }

                foreach (var item in group.QueryItems)
                {
                    string title;
                    string subTitle;

                    // If no name provided, use query as title and leave subtitle empty
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        title = item.Query;
                        subTitle = "";

                    // Otherwise use name as the title and query as subtitle
                    } else {
                        title = item.Name;
                        subTitle = item.Query;
                    }

                    if (title.ToLower().Contains(itemQuery.ToLower()))
                    {

                        results.Add(new Result
                        {
                            Title = title,
                            SubTitle = subTitle,
                            IcoPath = "Assets/query-icon.png",
                            ContextData = item,
                            Action = _ =>
                            {
                                // Change to the selected query item's set search query
                                _context.API.ChangeQuery(item.Query);
                                return false;
                            }
                        });
                    }
                }
            }

            return results;
        }

        private List<Result> GetGroupsResults(string queryString)
        {
            List<Result> results = new List<Result>();

            var queryFilter = queryString.ToLower();

            foreach (var group in _settings.QueryGroups)
            {
                // get names of all the items in the group
                List<string> itemNamesInGroup = group.QueryItems
                    .Select((i)=>i.Name)
                    .ToList();
                

                bool doesItemNameMatch = itemNamesInGroup.Any(
                    name => name.Contains(queryFilter, StringComparison.OrdinalIgnoreCase)
                );

                bool doesGroupNameMatch = group.Name.ToLower().Contains(queryFilter);

                // check if the group name or one of its items matches the query
                if (doesItemNameMatch || doesGroupNameMatch)
                {
                    // if prioritization is enabled, 
                    // calculate a bonus score based on how well the group name matches the query
                    // otherwise, (bonus) score is 0
                    int score = 0;
                    if (_settings.PrioritizeGroupResults && doesGroupNameMatch)
                        score = PrioritizedScoring(queryString, group.Name);

                    // make string from item names
                    string itemNamesInGroupString = string.Join(", ",itemNamesInGroup);

                    results.Add(new Result
                    {
                        Title = group.Name,
                        SubTitle = itemNamesInGroupString,
                        IcoPath = "Assets/icon.png",
                        Score = score, // either 0 or the prioritized score
                        ContextData = group,
                        Action = _ =>
                        {
                            // Change to the selected group's search query
                            _context.API.ChangeQuery(new SearchGroupCommandDefinition().BuildQuery(
                                pluginKeyword: mainPluginKeyword,
                                queryGroup:  group.Name
                            ), false);
                            return false;
                        }
                    });
                }
            }
            return results;
        }

        private Result GetAddGroupResult()
        {
            return new Result
            {
                Title = "Add New Group",
                SubTitle = "Create a new query group",
                Glyph = new GlyphInfo("sans-serif","＋"),
                Score = -100, // Low score to appear at the bottom (make sure real matches come first)
                Action = _ =>
                {
                    // Change to the add group query
                    _context.API.ChangeQuery(new AddGroupCommandDefinition().BuildQuery(
                        pluginKeyword: mainPluginKeyword
                    ), false);
                    return false;
                }
            };
        }
        private Result GetAddItemResult(string selectedGroup)
        {
            return new Result
            {
                Title = "Add New Query",
                SubTitle = "Add a new query to this group",
                Glyph = new GlyphInfo("sans-serif","＋"),
                Score = -100, // Low score to appear at the bottom (make sure real matches come first)
                Action = _ =>
                {
                    // Change to the add item query for the selected group
                    _context.API.ChangeQuery(new AddItemCommandDefinition().BuildQuery(
                        pluginKeyword: mainPluginKeyword,
                        queryGroup: selectedGroup
                    ), false);

                    return false;
                }
            };
        }

        private List<Result> GetAddGroupResults(string queryString)
        {
            // Explain to user they need to start
            if (string.IsNullOrEmpty(queryString))
            {
                return new List<Result>
                {
                    new Result
                    {
                        Title = "Add:",
                        SubTitle = "Start typing!",
                        Glyph = new GlyphInfo("sans-serif","＋"),
                    }
                };
            }

            // show error if name is not valid, eg is duplicate
            if (! _settings.isNewGroupNameValid(queryString))
            {
                return new List<Result>
                {
                    new Result
                    {
                        Title = "Invalid Name!",
                        SubTitle = queryString,
                        Glyph = new GlyphInfo("sans-serif","X"),
                    }
                };
            }

            return new List<Result>
            {
                new Result
                {
                    Title = "Add: " + queryString,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","＋"),
                    Score = 0,
                    Action = _ =>
                    {
                        _settings.AddGroup(Name:queryString);
                        _context.API.SavePluginSettings();
                        
                        // Change to the new group's search query
                        _context.API.ChangeQuery(new SearchGroupCommandDefinition().BuildQuery(
                            pluginKeyword: mainPluginKeyword,
                            queryGroup: queryString
                        ), false);

                        return false;
                    }
                }
            };
        }   

        private List<Result> GetAddItemResults(string selectedGroup, string itemQuery)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Add Query: " + itemQuery,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","＋"),
                    Action = _ =>
                    {
                        var group = _settings.QueryGroups.FirstOrDefault(g => g.Name == selectedGroup);
                        if (group is null) return false;

                        string itemName = group.isNewItemNameValid(itemQuery) 
                            ? itemQuery 
                            : group.GetNextDefaultItemName();
                        
                        group.AddItem(Name:itemName,Query:itemQuery);

                        _context.API.SavePluginSettings();
                        
                        // change to rename query for the new item
                        _context.API.ChangeQuery(new RenameItemCommandDefinition().BuildQuery(
                            pluginKeyword: mainPluginKeyword,
                            queryGroup: group.Name,
                            queryItem: itemName,
                            newItemName: itemName
                        ), false);

                        return false;
                    }
                }
            };
        }

        private List<Result> GetRenameGroupResults(string selectedGroup, string newName)
        {
            var group =_settings.QueryGroups.FirstOrDefault(g => g.Name == selectedGroup);

            // show error if name is not valid, eg is duplicate
            if (selectedGroup!=newName && ! _settings.isNewGroupNameValid(newName))
            {
                return new List<Result>
                {
                    new Result
                    {
                        Title = "Invalid Name!",
                        SubTitle = newName,
                        Glyph = new GlyphInfo("sans-serif","X"),
                    }
                };
            }

            return new List<Result>
            {
                new Result
                {
                    Title = "Set name to: " + newName,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","N"),
                    Action = _ =>
                    {
                        
                        if (group is not null)
                        {
                            group.Name = newName;
                        }

                        _context.API.SavePluginSettings();

                        // Go back to group search and filter by the new name
                        _context.API.ChangeQuery(new SearchGroupsCommandDefinition().BuildQuery(
                            pluginKeyword: mainPluginKeyword,
                            groupSearch: newName
                        ), false);

                        return false;
                    }
                }
            };
        }   

        private List<Result> GetRenameItemResults(string selectedGroup, string selectedItem, string newItemName)
        {
            var group =_settings.QueryGroups.FirstOrDefault(g => g.Name == selectedGroup);

            // show error if name is not valid, eg is duplicate
            if (selectedItem!=newItemName && ! group.isNewItemNameValid(newItemName))
            {
                return new List<Result>
                {
                    new Result
                    {
                        Title = "Invalid Name!",
                        SubTitle = newItemName,
                        Glyph = new GlyphInfo("sans-serif","X"),
                    }
                };
            }

            return new List<Result>
            {
                new Result
                {
                    Title = "Set name to: " + newItemName,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","N"),
                    Action = _ =>
                    {
                       
                        group.RenameItem(selectedItem,newItemName);

                        _context.API.SavePluginSettings();

                        // Go back to the group search query filtered for the modified item
                        _context.API.ChangeQuery(new SearchGroupCommandDefinition().BuildQuery(
                            pluginKeyword: mainPluginKeyword,
                            queryGroup: selectedGroup,
                            querySearch: newItemName
                        ), false);

                        return false;
                    }
                }
            };
        }
        private List<Result> GetSetItemQueryResults(string selectedGroup, string selectedItem, string newItemQuery)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Set query to: " + newItemQuery,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","Q"),
                    Action = _ =>
                    {
                        var group =_settings.QueryGroups.FirstOrDefault(g => g.Name == selectedGroup);
                        var item = group?.QueryItems.FirstOrDefault(i => i.Name == selectedItem);
                        if (item is not null)
                        {
                            item.Query = newItemQuery;
                        }

                        _context.API.SavePluginSettings();

                        // Go back to the group search query filtered for the modified item
                        _context.API.ChangeQuery(new SearchGroupCommandDefinition().BuildQuery(
                            pluginKeyword: mainPluginKeyword,
                            queryGroup: selectedGroup,
                            querySearch: selectedItem
                        ), false);

                        return false;
                    }
                }
            };
        }

        private int PrioritizedScoring(string query, string target, int maxScore=1000)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(target))
                return 0;

            query = query.ToLower();
            target = target.ToLower();

            // Compute the percentage of the target string that the query occupies.
            // Example: query="dev" (3) and target="development" (11) -> 3/11 ~= 27%
            double matchRatio = (double)query.Length / Math.Max(1, target.Length);

            // Scale this ratio to the scoring range.
            int score = (int)Math.Round(matchRatio * maxScore);

            return score;
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var results = new List<Result>();

            switch (selectedResult.ContextData)
            {
                case QueryGroup queryGroup:
                {
                    results.Add(new Result
                    {
                        Title = "Delete Group",
                        SubTitle = "Delete this query group",
                        Glyph = new GlyphInfo("sans-serif","X"),
                        Action = _ =>
                        {
                            _settings.QueryGroups.Remove(queryGroup);
                            _context.API.SavePluginSettings();
                            
                            _context.API.ReQuery();

                            return false;
                        }
                    });
                    results.Add(new Result
                    {
                        Title = "Change Name",
                        SubTitle = "Current name: " + queryGroup.Name,
                        Glyph = new GlyphInfo("sans-serif","N"),
                        Action = _ =>
                        {
                            _context.API.ReQuery();

                            // Change to the rename group query for the selected group
                            _context.API.ChangeQuery(new RenameGroupCommandDefinition().BuildQuery(
                                pluginKeyword: mainPluginKeyword,
                                queryGroup: queryGroup.Name,
                                newGroupName: queryGroup.Name
                            ), false);
                            
                            return false;
                        }
                    });
                    return results;
                }
                
                
                case QueryItem queryItem:
                {
                    
                    results.Add(new Result
                    {
                        Title = "Delete",
                        SubTitle = "Delete this query item",
                        Glyph = new GlyphInfo("sans-serif","X"),
                        Action = _ =>
                        {
                            _settings.QueryGroups.FirstOrDefault((QueryGroup qg)=>
                                qg.QueryItems.Contains(queryItem)
                            )
                            .QueryItems.Remove(queryItem);
                            _context.API.SavePluginSettings();

                            _context.API.ReQuery();

                            return false;
                        }
                    });
                    results.Add(new Result
                    {
                        Title = "Change Name",
                        SubTitle = "Current name: " + queryItem.Name,
                        Glyph = new GlyphInfo("sans-serif","N"),
                        Action = _ =>
                        {
                            var parentGroup = _settings.QueryGroups.FirstOrDefault((QueryGroup qg)=>
                                qg.QueryItems.Contains(queryItem)
                            );

                            _context.API.ReQuery();

                            // Change to the rename item query for the selected item
                            _context.API.ChangeQuery(new RenameItemCommandDefinition().BuildQuery(
                                pluginKeyword: mainPluginKeyword,
                                queryGroup: parentGroup.Name,
                                queryItem: queryItem.Name,
                                newItemName: queryItem.Name
                            ), false);
                            
                            return false;
                        }
                    });
                    results.Add(new Result
                    {
                        Title = "Change Query",
                        SubTitle = "Current query: " + queryItem.Query,
                        Glyph = new GlyphInfo("sans-serif","Q"),
                        Action = _ =>
                        {
                            var parentGroup = _settings.QueryGroups.FirstOrDefault((QueryGroup qg)=>
                                qg.QueryItems.Contains(queryItem)
                            );

                            _context.API.ReQuery();

                            // Change to the set item query query for the selected item
                            _context.API.ChangeQuery(new ItemQueryAssignmentCommandDefinition().BuildQuery(
                                pluginKeyword: mainPluginKeyword,
                                queryGroup: parentGroup.Name,
                                queryItem: queryItem.Name,
                                newItemQuery: queryItem.Query
                            ), false);
                            
                            return false;
                        }
                    });
                    return results;
                }

                default:
                {
                    return results;
                }
                
            }
        }
    }
}