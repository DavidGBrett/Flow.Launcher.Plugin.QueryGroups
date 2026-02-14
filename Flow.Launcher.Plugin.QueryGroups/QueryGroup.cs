using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroup : BaseModel
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

        public ObservableCollection<QueryItem> QueryItems { get; set; } = new ObservableCollection<QueryItem>
        {

        };
        
        public QueryItem AddItem(string Name=null,string Query="")
        {
            Name = Name ?? GetNextDefaultItemName();

            if (! isNewItemNameValid(Name))
            {
                throw new ArgumentException($"Invalid Name:{Name}");
            }

            var newItem = new QueryItem(
                Name:Name,
                Query:Query
            );

            QueryItems.Add(newItem);

            return newItem;
        }

        public void RenameItem(string existingName, string newName)
        {
            // stop if no change needed
            if (existingName == newName) return;

            if (! isNewItemNameValid(newName))
            {
                throw new ArgumentException($"Invalid Name:{newName}");
            }

            var item = QueryItems.FirstOrDefault(i => i.Name == existingName);

            item.Name = newName;
        }

        public bool isNewItemNameValid(string Name)
        {
            if (! QueryItem.isValidName(Name)) 
                return false;

            if (QueryItems.Any(i => i.Name == Name )) 
                return false;

            return true;
        }

        public string GetNextDefaultItemName()
        {
            int i = 0;
            string defaultPrefix = "query";
            string itemName;
            
            do {
                i+=1;
                itemName = $"{defaultPrefix}{i}";
            }
            while (QueryItems.Any(i => i.Name == itemName ));

            return itemName;
        }

        public static bool IsGroupNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (name.Contains(PluginConstants.QuerySeparator))
            {
                return false;
            }

            return true;
        }

    }
}