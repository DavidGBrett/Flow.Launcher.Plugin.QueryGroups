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

            // This means the user is looking for items in a group
            if (query.Search.StartsWith(groupSpecifierKeyword + QuerySeparator))
            {
                
                string queryAfterKeywordAndSep = query.Search.Substring(groupSpecifierKeyword.Length + QuerySeparator.Length);

                int numSeparators = query.Search.Split(QuerySeparator).Length - 1;

                // there is already a minimum of one separator here due to the startswith check
                // but having exactly one seperator means the user likely backspaced in the query after selecting a group
                // so we should just show the list of all groups again 
                // and include whatever was left in the query as a filter on it (without the seperators)
                // this will allow backspace to switch back to group selection mode
                if (numSeparators == 1)
                {
                    _context.API.ChangeQuery(groupSpecifierKeyword + " " + queryAfterKeywordAndSep);

                    return new List<Result>();
                }

                // if there are at least two separators the user is looking for items in a group
                else
                {
                    var selectedGroup = queryAfterKeywordAndSep.Split(QuerySeparator)[0];
                    var itemQuery = queryAfterKeywordAndSep.Substring(selectedGroup.Length + QuerySeparator.Length);

                    return GetGroupItemsResults(selectedGroup, itemQuery);
                }
            }
            
            // Otherwise, show the list of groups
            return GetGroupsResults(query.Search);
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
                            IcoPath = "images/query.png",
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
                        IcoPath = "images/icon.png",
                        Score = score, // either 0 or the prioritized score
                        Action = _ =>
                        {

                            var pluginID = _context.CurrentPluginMetadata.ID;

                            _context.API.ChangeQuery(groupSpecifierKeyword + QuerySeparator + group.Name + QuerySeparator, false);
                            return false;
                        }
                    });
                }
            }

            return results;
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