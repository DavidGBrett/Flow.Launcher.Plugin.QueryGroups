using System;

namespace Flow.Launcher.Plugin.QueryGroups
{
    
    public class QueryItem : BaseModel
    {
        private string _name;

        public string Name {
            set => _name = value;
            get {
                    if (string.IsNullOrEmpty(_name)) return Query;
                    else return _name;
                }
        }

        public string Query { get; set; }

    }
}