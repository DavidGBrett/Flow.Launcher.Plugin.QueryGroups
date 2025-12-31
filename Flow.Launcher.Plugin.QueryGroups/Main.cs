using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroups : IPlugin, ISettingProvider
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
            AddItem
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
                    results.Add(GetAddGroupResult());
                    return results;
                    }

                case PluginQueryType.SearchGroups:
                    {
                        string groupQuery = queryParts.Count > 0 ? queryParts[0] : "";
                        return GetGroupsResults(groupQuery);
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

                    int score = 0;
                    if (_settings.PrioritizeGroupResults)
                        score = PrioritizedScoring(queryString, group.Name);

                    results.Add(new Result
                    {
                        Title = group.Name,
                        SubTitle = groupSpecifierKeyword,
                        IcoPath = "Assets/icon.png",
                        Score = score, // either 0 or the prioritized score
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
                IcoPath = "Assets/icon.png",
                Score = 0,
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
                IcoPath = "Assets/icon.png",
                Score = 0,
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
                    IcoPath = "Assets/icon.png",
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
                    IcoPath = "Assets/icon.png",
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
    }
}