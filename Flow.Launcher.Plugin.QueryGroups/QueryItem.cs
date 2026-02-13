using System;

namespace Flow.Launcher.Plugin.QueryGroups
{
    
    public class QueryItem : BaseModel
    {
        private string _name;

        public string Name {
            set {
                if( !isValidName(value))
                {
                    throw new ArgumentException($"Invalid Name:{value}");
                }

                _name = value;
            }
            get {
                return _name;
            }
        }

        public string Query { get; set; }

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