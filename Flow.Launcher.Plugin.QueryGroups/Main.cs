using System;
using System.Collections.Generic;
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

        public List<Result> Query(Query query)
        {
            // if the query does not start with the group specifier keyword, show the list of groups
            if (!query.Search.StartsWith(groupSpecifierKeyword + QuerySeparator))
            {
                return GetGroupsResults(query.Search);
            }

            // Otherwise this means the user is looking for items in a group

            // extract the part after the keyword-, should be in the form of "GroupName-ItemQuery" or just "GroupName-"
            string groupItemQuery = query.Search.Substring(groupSpecifierKeyword.Length + QuerySeparator.Length);

            // when selecting a group, the groupItemQuery will initialy be in the form of "GroupName-"
            // so if there is no separator after the group name that means the user backspaced after selecting a group
            // so we rephrase the query to return to group selection mode
            if (!groupItemQuery.Contains(QuerySeparator))
            {
                _context.API.ChangeQuery(groupSpecifierKeyword + " " + groupItemQuery);
                return new List<Result>();
            }

            // Now we can safely split the groupItemQuery into group name and item query

            // everything before first separator is the group name
            var selectedGroup = groupItemQuery.Split(QuerySeparator)[0];

            // everything after the first separator is the item query (written this way to preserve any additional separators in the item query)
            var itemQuery = groupItemQuery.Substring(selectedGroup.Length + QuerySeparator.Length);

            // if the selected group is "Add", then they actually want to add a new group
            if (selectedGroup == "Add") return GetAddGroupResults(itemQuery);

            return GetGroupItemsResults(selectedGroup, itemQuery);
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
                        Action = _ =>
                        {
                            _context.API.ChangeQuery(groupSpecifierKeyword + QuerySeparator + group.Name + QuerySeparator, false);
                            return false;
                        }
                    });
                }
            }

            // Always include the "Add Group" option at the end
            results.Add(new Result
            {
                Title = "Add Query Group",
                SubTitle = "",
                IcoPath = "Assets/icon.png",
                Score = 0,
                Action = _ =>
                {
                    
                    _context.API.ChangeQuery(groupSpecifierKeyword + QuerySeparator + "Add" + QuerySeparator, false);
                    return false;
                }
            });

            return results;
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
                        _context.API.ChangeQuery(groupSpecifierKeyword + QuerySeparator + queryString + QuerySeparator, false);
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