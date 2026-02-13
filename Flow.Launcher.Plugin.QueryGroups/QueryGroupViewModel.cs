using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroupViewModel: BaseModel
    {
        public QueryGroup QueryGroup;
        public ObservableCollection<QueryItemViewModel> QueryItemVMs {get; set;}

        private SettingsViewModel _settingsVM;


        public ICommand AddItemCommand { get; }
        
        public ICommand DeleteItemCommand { get; }

        private string _editName;

        private bool _isEditNameInvalid = false;
        public bool isEditNameInvalid {
            get { return _isEditNameInvalid;} 
            private set
            {
                _isEditNameInvalid = value;
                OnPropertyChanged();
            }
        }

        public string EditName {
            get {return _editName;}
            set
            {
                _editName = value;

                if (_editName == QueryGroup.Name)
                {
                    isEditNameInvalid = false;
                }
                
                else if (_settingsVM.Settings.isNewGroupNameValid(EditName))
                {
                    QueryGroup.Name = _editName;
                    isEditNameInvalid = false;
                }
                else
                {
                    isEditNameInvalid = true;
                }
            }
        }

         public QueryGroupViewModel(
            QueryGroup queryGroup,
            SettingsViewModel settingsVM
        )
        {
            _settingsVM = settingsVM;
            QueryGroup = queryGroup;
            EditName = queryGroup.Name;

            QueryItemVMs = new ObservableCollection<QueryItemViewModel>(
                queryGroup.QueryItems
                    .Select(qi => new QueryItemViewModel(qi, this)));

            AddItemCommand = new RelayCommand(AddItem);
            DeleteItemCommand = new RelayCommand<QueryItemViewModel>(DeleteItem);

            queryGroup.QueryItems.CollectionChanged += OnQueryItemsChanged;
        }


        private void AddItem()
        {
            QueryGroup.AddItem();
        }

        private void DeleteItem(QueryItemViewModel itemVM)
        {
            QueryGroup.QueryItems.Remove(itemVM.QueryItem);
        }

        private void OnQueryItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems == null || QueryGroup.QueryItems.Count == QueryItemVMs.Count)
                    {
                        return;
                    }

                    foreach (QueryItem newItem in e.NewItems)
                    {
                        QueryItemVMs.Add(new QueryItemViewModel(newItem,this));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems == null || QueryGroup.QueryItems.Count == QueryItemVMs.Count)
                    {
                        return;
                    }

                    foreach (QueryItem oldItem in e.OldItems)
                    {
                        RemoveQueryItemVMByModel(oldItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (QueryItem oldItem in e.OldItems)
                    {
                        RemoveQueryItemVMByModel(oldItem);
                    }
                    foreach (QueryItem newItem in e.NewItems)
                    {
                        QueryItemVMs.Add(new QueryItemViewModel(newItem,this));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    QueryItemVMs = new ObservableCollection<QueryItemViewModel>(
                        QueryGroup.QueryItems
                            .Select(qi => new QueryItemViewModel(qi, this)));
                    break;
            }
        }

        private void RemoveQueryItemVMByModel(QueryItem removedItem)
        {
            QueryItemViewModel toRemove = QueryItemVMs.FirstOrDefault((vm)=> vm.QueryItem.Name == removedItem.Name);
            QueryItemVMs.Remove(toRemove);
        }
    }
}