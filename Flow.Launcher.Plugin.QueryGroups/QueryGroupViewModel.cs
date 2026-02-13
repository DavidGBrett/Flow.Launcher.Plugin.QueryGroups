using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroupViewModel: BaseModel
    {
        public QueryGroup QueryGroup;

        public ObservableCollection<QueryItem> QueryItems {get; set;}

        private SettingsViewModel _settings;

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
            QueryItems = queryGroup.QueryItems;
        }
    }
}