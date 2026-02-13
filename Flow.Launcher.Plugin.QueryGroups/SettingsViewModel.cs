using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class SettingsViewModel: BaseModel
    {
        public ICommand DeleteGroupCommand { get; }
        public ICommand AddGroupCommand { get; }

        public ObservableCollection<QueryGroupViewModel> QueryGroups {get; set;}

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;

            QueryGroups = new ObservableCollection<QueryGroupViewModel>(
                Settings.QueryGroups
                    .Select(g => new QueryGroupViewModel(g, this)));

            AddGroupCommand = new RelayCommand(AddGroup);
            DeleteGroupCommand = new RelayCommand<QueryGroupViewModel>(DeleteGroup);

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
    }
}