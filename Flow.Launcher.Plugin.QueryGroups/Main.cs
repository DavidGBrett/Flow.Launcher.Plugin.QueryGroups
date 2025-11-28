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
        private string seperator;

        public void Init(PluginInitContext context)
        {
            _context = context;

            _settings = _context.API.LoadSettingJsonStorage<Settings>();
            _viewModel = new SettingsViewModel(_settings);

            groupSpecifierKeyword = _context.CurrentPluginMetadata.ActionKeywords[0];
            seperator = "-";
        }

        public Control CreateSettingPanel()
        {
            return new SettingsControl(_context, _viewModel);
        }

        public List<Result> Query(Query query)
        {

            if (query.Search.StartsWith(groupSpecifierKeyword))
            {
                return GetGroupItemsResults(query);
            }
            else
            {
                return GetGroupsResults(query);
            }
        }

        private List<Result> GetGroupItemsResults(Query query)
        {
            List<Result> results = new List<Result>();

            var queryAfterKeyword = query.Search.Substring(groupSpecifierKeyword.Length + seperator.Length);
            var selectedGroup = queryAfterKeyword.Split(seperator)[0];
            var itemQuery = queryAfterKeyword.Substring(selectedGroup.Length + seperator.Length);

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
                            // IcoPath = "Images/icon.png",
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

        private List<Result> GetGroupsResults(Query query)
        {
            List<Result> results = new List<Result>();

            foreach (var group in _settings.QueryGroups)
            {
                if (group.Name.ToLower().Contains(query.Search.ToLower()))
                {

                    int score = 0;
                    if (_settings.PrioritizeGroupResults)
                        score = PrioritizedScoring(query.Search, group.Name);

                    results.Add(new Result
                    {
                        Title = group.Name,
                        // SubTitle = $"Group containing: {string.Join(", ", group.Value)}",
                        SubTitle = groupSpecifierKeyword,
                        IcoPath = "Images/icon.png",
                        Score = score, // so it appears on top
                        Action = _ =>
                        {
                            // Define what happens when the result is selected
                            // _context.API.ShowMsg($"Selected group: {group.Key}");

                            var pluginID = _context.CurrentPluginMetadata.ID;


                            // _context.API.AddActionKeyword(pluginID, tempKeyword);
                            _context.API.ChangeQuery(groupSpecifierKeyword + seperator + group.Name + seperator, false);
                            // _context.API.ReQuery();
                            // _context.API.RemoveActionKeyword(pluginID, tempKeyword);

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