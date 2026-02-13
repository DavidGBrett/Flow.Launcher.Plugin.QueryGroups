using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class Settings : BaseModel
    {
        public Settings()
        {

        }

        public bool PrioritizeGroupResults { get; set; } = true;

        public ObservableCollection<QueryGroup> QueryGroups { get; set; } = new ObservableCollection<QueryGroup>
        {
            
        };

        public bool isNewGroupNameValid(string newGroupName)
        {   
            if (string.IsNullOrWhiteSpace(newGroupName))
            {
                return false;
            }

            if (newGroupName.Contains("-"))
            {
                return false;
            }

            if (QueryGroups.Any(qg => qg.Name == newGroupName)){
                return false;
            }

            return true;
        }


    }
}