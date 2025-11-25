using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Flow.Launcher.Plugin.CustomGroups
{
    public class Settings : BaseModel
    {
        public Settings()
        {

        }

        public bool PrioritizeGroupResults { get; set; } = true;

        public ObservableCollection<QueryGroup> QueryGroups { get; set; } = new ObservableCollection<QueryGroup>
        {
            
            // new QueryGroup{
            //     Name = "AI Chat",
            //     QueryItems = new ObservableCollection<QueryItem>{
            //         new QueryItem{
            //             Name = "ChatGPT2",
            //             Query = "ChatGPT"
            //         },
            //         new QueryItem{
            //             Name = "Deepseek",
            //             Query = "Deepseek"
            //         },
            //         new QueryItem{
            //             Name = "Gemini",
            //             Query = "g Gemini"
            //         }
            //     }
            // }
        };


    }
}