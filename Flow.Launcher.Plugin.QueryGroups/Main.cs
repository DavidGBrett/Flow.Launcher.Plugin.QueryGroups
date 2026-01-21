using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroups : IPlugin, ISettingProvider, IContextMenu
    {
        private PluginInitContext _context;

        private Settings _settings;
        private SettingsViewModel _viewModel;

        private string groupSpecifierKeyword;
        private const string QuerySeparator = "-";

        public void Init(PluginInitContext context)
        {
            _context = context;

            _settings = _context.API.LoadSettingJsonStorage<Settings>();
            _viewModel = new SettingsViewModel(_settings);

            groupSpecifierKeyword = _context.CurrentPluginMetadata.ActionKeywords[1];
        }

        public Control CreateSettingPanel()
        {
            return new SettingsControl(_context, _viewModel);
        }

        private enum PluginQueryType
        {
            Keywordless,
            SearchGroups,
            AddGroup,
            SearchGroup,
            AddItem,
            RenameGroup
        }

        public List<Result> Query(Query query)
        {
            // Split the query into parts based on the separator
            var queryParts = new List<string>(query.Search.Split(new string[] { QuerySeparator }, StringSplitOptions.None));
            
            // Determine the type of query
            var queryType = MatchQueryType(query, queryParts);

            // Handle the query based on its type
            switch (queryType)
            {
                case PluginQueryType.Keywordless:
                    {
                    List<Result> results = GetGroupsResults(query.Search);
                    return results;
                    }

                case PluginQueryType.SearchGroups:
                    {
                        string groupQuery = queryParts.Count > 0 ? queryParts[0] : "";
                        List<Result> results = GetGroupsResults(groupQuery);
                        results.Add(GetAddGroupResult());

                        return results;
                    }

                case PluginQueryType.AddGroup:
                    {
                        string newGroupName = queryParts.Count > 1 ? queryParts[1] : "";
                        return GetAddGroupResults(newGroupName);
                    }

                case PluginQueryType.SearchGroup:
                    {
                        string selectedGroup = queryParts[0];
                        string itemQuery = queryParts.Count > 1 ? queryParts[1] : "";

                        List<Result> results = GetGroupItemsResults(selectedGroup, itemQuery);
                        results.Add(GetAddItemResult(selectedGroup));

                        return results;
                    }

                case PluginQueryType.AddItem:
                    {
                        string selectedGroup = queryParts[0];
                        string itemQuery = queryParts.Count > 2 ? queryParts[2] : "";
                        return GetAddItemResults(selectedGroup, itemQuery);
                    }

                case PluginQueryType.RenameGroup:
                    {
                        string selectedGroup = queryParts[0];
                        string newName = queryParts.Count > 2 ? queryParts[2] : "";
                        return GetRenameGroupResults(selectedGroup,newName);
                    }

                default:
                    return new List<Result>();
            }
        }



        private PluginQueryType MatchQueryType(Query query, List<string> queryParts)
        {
            if (query.ActionKeyword != groupSpecifierKeyword)
            {
                return PluginQueryType.Keywordless;
            }
            
            if (string.IsNullOrEmpty(queryParts[0]))
            {
                return PluginQueryType.SearchGroups;
            }

            if (queryParts[0] == "Add")
            {
                return PluginQueryType.AddGroup;
            }
            
            if (queryParts.Count == 1)
            {
                return PluginQueryType.SearchGroups;
            }

            if (queryParts[1] == "Add")
            {
                return PluginQueryType.AddItem;
            }
            if (queryParts[1] == "Rename")
            {
                return PluginQueryType.RenameGroup;
            }

            return PluginQueryType.SearchGroup;

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

            foreach (var group in _settings.QueryGroups)
            {
                // check if group name matches query
                if (group.Name.ToLower().Contains(queryString.ToLower()))
                {
                    // if prioritization is enabled, 
                    // calculate a bonus score based on how well the group name matches the query
                    // otherwise, (bonus) score is 0
                    int score = 0;
                    if (_settings.PrioritizeGroupResults)
                        score = PrioritizedScoring(queryString, group.Name);

                    results.Add(new Result
                    {
                        Title = group.Name,
                        SubTitle = groupSpecifierKeyword,
                        IcoPath = "Assets/icon.png",
                        Score = score, // either 0 or the prioritized score
                        ContextData = group,
                        Action = _ =>
                        {

                            var pluginID = _context.CurrentPluginMetadata.ID;

                            _context.API.ChangeQuery(groupSpecifierKeyword + " " + group.Name + QuerySeparator, false);
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
                    _context.API.ChangeQuery(groupSpecifierKeyword + " Add" + QuerySeparator, false);
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
                    _context.API.ChangeQuery(groupSpecifierKeyword +" "+selectedGroup + QuerySeparator+"Add"+QuerySeparator, false);
                    return false;
                }
            };
        }

        private List<Result> GetAddGroupResults(string queryString)
        {
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
                        _settings.QueryGroups.Add(new QueryGroup { Name = queryString });
                        _context.API.SavePluginSettings();
                        _context.API.ChangeQuery(groupSpecifierKeyword + " " + queryString + QuerySeparator, false);
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
                    Title = "Add: " + itemQuery,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","＋"),
                    Action = _ =>
                    {
                        _settings.QueryGroups.FirstOrDefault(g => g.Name == selectedGroup)
                        ?.QueryItems.Add(new QueryItem { Query = itemQuery });
                        _context.API.SavePluginSettings();
                        _context.API.ChangeQuery(groupSpecifierKeyword + " " + selectedGroup + QuerySeparator, false);
                        return false;
                    }
                }
            };
        }  
        private List<Result> GetRenameGroupResults(string selectedGroup, string newName)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Rename To: " + newName,
                    SubTitle = "",
                    Glyph = new GlyphInfo("sans-serif","R"),
                    Action = _ =>
                    {
                        var group =_settings.QueryGroups.FirstOrDefault(g => g.Name == selectedGroup);
                        if (group is not null)
                        {
                            group.Name = newName;
                        }

                        _context.API.SavePluginSettings();
                        _context.API.ChangeQuery(groupSpecifierKeyword + " " + newName + QuerySeparator, false);
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
                results.Add(new Result
                {
                    Title = "Delete Group",
                    SubTitle = "Delete this query group",
                    Glyph = new GlyphInfo("sans-serif"," X"),
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
                    Title = "Rename Group",
                    SubTitle = "Rename this query group",
                    Glyph = new GlyphInfo("sans-serif"," R"),
                    Action = _ =>
                    {
                        _context.API.ReQuery();
                        _context.API.ChangeQuery(groupSpecifierKeyword + " " + queryGroup.Name + QuerySeparator + "Rename" + QuerySeparator + queryGroup.Name, false);
                        
                        return false;
                    }
                });
                return results;
                
                case QueryItem queryItem:
                results.Add(new Result
                {
                    Title = "Delete Query",
                    SubTitle = "Delete this query item",
                    Glyph = new GlyphInfo("sans-serif"," X"),
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
                return results;

                default:
                return results;
            }
        }
    }
}