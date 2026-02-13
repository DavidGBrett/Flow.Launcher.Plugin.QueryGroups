using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;

namespace Flow.Launcher.Plugin.QueryGroups
{
    public class QueryGroup : BaseModel
    {
        public string Name { get; set; }

        public ObservableCollection<QueryItem> QueryItems { get; set; } = new ObservableCollection<QueryItem>
        {

        };

        public void AddItem(string Query="")
        {
            string nextDefaultName = GetNextDefaultItemName();
            
            AddItem(
                Name:nextDefaultName,
                Query:Query
            );
        }
        public void AddItem(string Name,string Query="")
        {
            if (! isNewItemNameValid(Name))
            {
                throw new ArgumentException($"Invalid Name:{Name}");
            }

            QueryItems.Add(new QueryItem(
                Name:Name,
                Query:Query
            ));
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

    }
}