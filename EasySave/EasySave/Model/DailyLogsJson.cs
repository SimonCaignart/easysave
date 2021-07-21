using System.Collections.Generic;

namespace EasySave
{
    /// <summary>
    /// Declares public variables that will be used in the daily log json files.
    /// </summary>
    class DailyLogsJson
    {
        public string Name;
        public string Timestamp;
        public string Activity;
        public string Source;
        public string Destination;
        public long FileSize;
        public double TransferTime;
    }

    /// <summary>
    /// Creates a list to add json content to the daily log files.
    /// </summary>
    class DailyLogList
    {
        public List<DailyLogsJson> DailyLog { get; set; }
    }
}