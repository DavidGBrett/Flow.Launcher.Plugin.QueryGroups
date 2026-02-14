using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryItemViewModel: BaseModel
    {
        public QueryItem QueryItem;

        public QueryGroupViewModel ParentGroupVM {get;set;}

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

                if (_editName == QueryItem.Name)
                {
                    isEditNameInvalid = false;
                }
                
                else if (ParentGroupVM.QueryGroup.isNewItemNameValid(EditName))
                {
                    QueryItem.Name = _editName;
                    isEditNameInvalid = false;
                }
                else
                {
                    isEditNameInvalid = true;
                }
            }
        }

        public string Query
        {
            get
            {
                return QueryItem.Query;
            }
            set
            {
                QueryItem.Query = value;
                OnPropertyChanged();
            }
        }

         public QueryItemViewModel(
            QueryItem queryItem,
            QueryGroupViewModel parentGroupVM
        )
        {
            QueryItem = queryItem;
            ParentGroupVM = parentGroupVM;
            EditName = queryItem.Name;

            QueryItem.PropertyChanged += OnItemPropChanged;
        }

        private void OnItemPropChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    // directly update the internal editName, so it doesnt try to edit Name again
                    _editName = QueryItem.Name;

                    // tell the ui it changed
                    OnPropertyChanged(nameof(EditName));
                    
                    break;
                    
                case "Query":
                    // Query just returns the model's query so we only need to tell the ui it changed
                    OnPropertyChanged(nameof(Query));
                    
                    break;
            }
        }
    }
}