using System.Collections.Generic;

namespace EasySave
{
    /// <summary>
    /// Declares public variables that will be used in the state log json file.
    /// </summary>
    class StateLogsJson
    {
        public string Name { get; set; }
        public string Timestamp;
        public string Type;
        public string Activity;
        public string IsActivelySaving;
        public string Source;
        public string Destination;
        public int? NumberOfFiles;
        public long TotalFileSize;
        public string Progression;
        public int? FilesRemaining;
        public string FilesSizeRemaining;
        public string EncryptionTime;
        public int IsPaused;
    }

    /// <summary>
    /// Creates a list to add json content to the state log file.
    /// </summary>
    class StateLogList
    {
        public List<StateLogsJson> StateLog { get; set; }
    }

}