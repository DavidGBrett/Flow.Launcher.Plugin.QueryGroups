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

            if (newGroupName.Contains(PluginConstants.QuerySeparator))
            {
                return false;
            }

            if (QueryGroups.Any(qg => qg.Name == newGroupName)){
                return false;
            }

            return true;
        }

        public QueryGroup AddGroup(string Name = null)
        {
            if (Name is null)
            {
                Name = GetNextDefaultGroupName();
            }
            else if(!isNewGroupNameValid(Name))
            {
                throw new ArgumentException($"Invalid Name:{Name}");
            }

            var newGroup = new QueryGroup
                {
                    Name=Name
                };

            QueryGroups.Add(
                newGroup
            );

            return newGroup;
        }

        public string GetNextDefaultGroupName()
        {
            int i = 0;
            string defaultPrefix = "group";
            string itemName;
            
            do {
                i+=1;
                itemName = $"{defaultPrefix}{i}";
            }
            while (QueryGroups.Any(g => g.Name == itemName ));

            return itemName;
        }
    }
}