using System;

namespace Flow.Launcher.Plugin.CustomGroups
{
    public enum QueryType
    {
        Query,
        OpenUri,
        Cmd
    }

    public static class EnumValues
    {
        public static Array QueryTypes => Enum.GetValues(typeof(QueryType));
    }
}