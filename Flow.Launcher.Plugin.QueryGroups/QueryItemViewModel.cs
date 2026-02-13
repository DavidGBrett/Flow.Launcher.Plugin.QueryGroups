using System.Collections.ObjectModel;

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
        }
    }
}