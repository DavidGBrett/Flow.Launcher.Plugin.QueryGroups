using System.Collections.ObjectModel;

namespace Flow.Launcher.Plugin.CustomGroups
{
    public class QueryGroup : BaseModel
    {
        public string Name { get; set; }

        public ObservableCollection<QueryItem> QueryItems { get; set; } = new ObservableCollection<QueryItem>
        {

        };

    }
}