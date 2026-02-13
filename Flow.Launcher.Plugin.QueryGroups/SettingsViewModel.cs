using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class SettingsViewModel: BaseModel
    {
        public ICommand AddItemCommand { get; }
        public ICommand DeleteGroupCommand { get; }
        
        public ICommand AddGroupCommand { get; }
        public ICommand DeleteItemCommand { get; }

        public ObservableCollection<QueryGroupViewModel> QueryGroups {get; set;}

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;

            QueryGroups = new ObservableCollection<QueryGroupViewModel>(
                Settings.QueryGroups
                    .Select(g => new QueryGroupViewModel(g, this)));

            AddGroupCommand = new RelayCommand(AddGroup);
            DeleteGroupCommand = new RelayCommand<QueryGroupViewModel>(DeleteGroup);

            AddItemCommand = new RelayCommand<QueryGroupViewModel>(AddItem);
            DeleteItemCommand = new RelayCommand<QueryItem>(DeleteItem);

        }

        public Settings Settings { get; }

        private void AddGroup()
        {
            var newGroup = Settings.AddGroup();
            QueryGroups.Add(new QueryGroupViewModel(newGroup,this));
        }

        private void DeleteGroup(QueryGroupViewModel group)
        {
            Settings.QueryGroups.Remove(group.QueryGroup);
            QueryGroups.Remove(group);
        }

        private void AddItem(QueryGroupViewModel group)
        {
            group.QueryGroup.AddItem();
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