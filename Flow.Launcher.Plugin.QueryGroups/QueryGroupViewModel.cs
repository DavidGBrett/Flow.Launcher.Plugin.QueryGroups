using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroupViewModel: BaseModel
    {
        public QueryGroup QueryGroup;

        public ObservableCollection<QueryItem> QueryItems {get; set;}

        public string EditName {get; set;}

         public QueryGroupViewModel(
            QueryGroup queryGroup,
            SettingsViewModel settings
        )
        {
            QueryGroup = queryGroup;
            EditName = queryGroup.Name;
            QueryItems = queryGroup.QueryItems;
        }
    }
}