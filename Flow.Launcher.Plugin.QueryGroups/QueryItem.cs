using System;

namespace Flow.Launcher.Plugin.QueryGroups
{
    
    public class QueryItem : BaseModel
    {
        private string _name;

        public string Name {
            set {
                _name = value;
                OnPropertyChanged();
            }
            get {
                return _name;
            }
        }

        private string _query;

        public string Query {
            set {
                _query = value;
                OnPropertyChanged();
            }
            get {
                return _query;
            }
        }

        public QueryItem(string Name,string Query="")
        {
            this.Name = Name;
            this.Query = Query;
        }

        public static bool isValidName(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name)){
                return false;
            }
            
            if (Name.Contains(PluginConstants.QuerySeparator))
            {
                return false;
            }

            return true;

        }

    }
}