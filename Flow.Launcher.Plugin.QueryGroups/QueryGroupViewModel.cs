using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroupViewModel: BaseModel
    {
        public QueryGroup QueryGroup;
        public ObservableCollection<QueryItemViewModel> QueryItems {get; set;}

        private SettingsViewModel _settings;


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
                
                else if (_settings.Settings.isNewGroupNameValid(EditName))
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
            SettingsViewModel settings
        )
        {
            _settings = settings;
            QueryGroup = queryGroup;
            EditName = queryGroup.Name;

            QueryItems = new ObservableCollection<QueryItemViewModel>(
                queryGroup.QueryItems
                    .Select(qi => new QueryItemViewModel(qi, this)));

            AddItemCommand = new RelayCommand(AddItem);
            DeleteItemCommand = new RelayCommand<QueryItemViewModel>(DeleteItem);
        }


        private void AddItem()
        {
            var newItem = QueryGroup.AddItem();
            QueryItems.Add(new QueryItemViewModel(newItem,this));
        }

        private void DeleteItem(QueryItemViewModel item)
        {
            QueryGroup.QueryItems.Remove(item.QueryItem);
            QueryItems.Remove(item);
        }
    }
}