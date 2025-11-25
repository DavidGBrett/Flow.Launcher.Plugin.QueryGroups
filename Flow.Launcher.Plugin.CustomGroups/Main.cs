using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.CustomGroups
{
    public class CustomGroups : IPlugin, ISettingProvider
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
                    if (item.Name.ToLower().Contains(itemQuery.ToLower()))
                    {
                        results.Add(new Result
                        {
                            Title = item.Name,
                            SubTitle = item.Query,
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
                    results.Add(new Result
                    {
                        Title = group.Name,
                        // SubTitle = $"Group containing: {string.Join(", ", group.Value)}",
                        SubTitle = groupSpecifierKeyword,
                        IcoPath = "Images/icon.png",
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
    }
}