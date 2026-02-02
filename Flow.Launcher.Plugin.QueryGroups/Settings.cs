using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class Settings : BaseModel
    {
        public Settings()
        {

        }

        public bool PrioritizeGroupResults { get; set; } = true;

        public string TermSeparator { get; set; } = "-";

        public ObservableCollection<QueryGroup> QueryGroups { get; set; } = new ObservableCollection<QueryGroup>
        {
            
        };


    }
}