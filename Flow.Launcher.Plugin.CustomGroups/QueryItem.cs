using System;

namespace Flow.Launcher.Plugin.CustomGroups
{
    
    public class QueryItem : BaseModel
    {
        

        // public static Array QueryTypeValues => Enum.GetValues(typeof(QueryType));

        public string Name { get; set; }

        public string Query { get; set; }

        public QueryType Type { get; set; }

    }
}