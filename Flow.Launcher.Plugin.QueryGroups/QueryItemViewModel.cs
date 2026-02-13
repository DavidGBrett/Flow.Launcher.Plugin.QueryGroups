using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryItemViewModel: BaseModel
    {
        public QueryItem QueryItem;

        public QueryGroupViewModel ParentGroup;

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
                
                else if (ParentGroup.QueryGroup.isNewItemNameValid(EditName))
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
            QueryGroupViewModel parentGroup
        )
        {
            QueryItem = queryItem;
            ParentGroup = parentGroup;
            EditName = queryItem.Name;
        }
    }
}