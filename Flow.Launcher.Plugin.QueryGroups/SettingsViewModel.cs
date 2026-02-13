using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public ObservableCollection<QueryGroupViewModel> QueryGroupVMs {get; set;}

        public bool PrioritizeGroupResults {
            get
            {
                return Settings.PrioritizeGroupResults;
            }
            set
            {
                Settings.PrioritizeGroupResults = value;
            }
        }

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;

            QueryGroupVMs = new ObservableCollection<QueryGroupViewModel>(
                Settings.QueryGroups
                    .Select(g => new QueryGroupViewModel(g, this)));

            AddGroupCommand = new RelayCommand(AddGroup);
            DeleteGroupCommand = new RelayCommand<QueryGroupViewModel>(DeleteGroup);

            Settings.QueryGroups.CollectionChanged += OnQueryGroupsChanged;
        }

        public Settings Settings { get; }

        private void AddGroup()
        {
            Settings.AddGroup();
        }

        private void DeleteGroup(QueryGroupViewModel groupVM)
        {
            Settings.QueryGroups.Remove(groupVM.QueryGroup);
        }

        private void OnQueryGroupsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null || Settings.QueryGroups.Count == QueryGroupVMs.Count)
                    {
                        return;
                    }

                    foreach (QueryGroup newGroup in e.NewItems)
                    {
                        QueryGroupVMs.Add(new QueryGroupViewModel(newGroup,this));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null || Settings.QueryGroups.Count == QueryGroupVMs.Count)
                    {
                        return;
                    }

                    foreach (QueryGroup oldGroup in e.OldItems)
                    {
                        QueryGroupViewModel toRemove = QueryGroupVMs.FirstOrDefault((qg)=> qg.QueryGroup.Name == oldGroup.Name);
                        QueryGroupVMs.Remove(toRemove);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (QueryGroup oldGroup in e.OldItems)
                    {
                        QueryGroupViewModel toRemove = QueryGroupVMs.FirstOrDefault((qg)=> qg.QueryGroup.Name == oldGroup.Name);
                        QueryGroupVMs.Remove(toRemove);
                    }
                    foreach (QueryGroup newGroup in e.NewItems)
                    {
                        QueryGroupVMs.Add(new QueryGroupViewModel(newGroup,this));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    QueryGroupVMs = new ObservableCollection<QueryGroupViewModel>(
                        Settings.QueryGroups
                            .Select(g => new QueryGroupViewModel(g, this)));
                    break;
            }
        }
    }
}