using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class SettingsViewModel
    {
        public ICommand AddItemCommand { get; }
        public ICommand DeleteGroupCommand { get; }
        
        public ICommand AddGroupCommand { get; }
        public ICommand DeleteItemCommand { get; }

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;

            AddGroupCommand = new RelayCommand(AddGroup);
            DeleteGroupCommand = new RelayCommand<QueryGroup>(DeleteGroup);

            AddItemCommand = new RelayCommand<QueryGroup>(AddItem);
            DeleteItemCommand = new RelayCommand<QueryItem>(DeleteItem);

        }

        public Settings Settings { get; }

        private void AddGroup()
        {
            Settings.QueryGroups.Add(
                new QueryGroup()
            );
        }

        private void DeleteGroup(QueryGroup group)
        {
            // if (group is not null)
            Settings.QueryGroups.Remove(group);
        }

        private void AddItem(QueryGroup group)
        {
            group.QueryItems.Add(new QueryItem());
        }

        private void DeleteItem(QueryItem item)
        {
            item.Name = "Deleting...";
            var group = Settings.QueryGroups.FirstOrDefault(g => g.QueryItems.Contains(item));
            if (group != null)
                group.QueryItems.Remove(item);

            
        }
    }
}