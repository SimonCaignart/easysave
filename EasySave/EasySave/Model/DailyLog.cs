using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace EasySave
{

    /// <summary>
    /// Main daily log class.
    /// Contains all the methods required to create the daily log files and add all the necessary information.
    /// </summary>
    class DailyLog
    {
        public string Data;
        Mutex _m = new Mutex(false, "writeFile");
        public DailyLogList Dailylogfile { get; set; }
        /// <summary>
        /// Creates the daily log file in the folder created for it.
        /// </summary>
        public string dailylogPath = Path.GetFullPath(@"Logs\dailylogs\dailyLog" + DateTime.Now.ToString("dd-MM-yyyy") + ".json");

        /// <summary>
        /// Calls the daily log list and creates the daily log folder.
        /// </summary>
        public DailyLog()
        {
            Dailylogfile = new DailyLogList();
            Directory.CreateDirectory(Path.GetFullPath(@"Logs\dailylogs"));
        }

        /// <summary>
        /// Gives a value to all the information required for the build of the daily log files.
        /// </summary>
        /// <returns>
        /// A daily log file per day.
        /// </returns>
        /// <param name="SaveName"></param>
        /// <param name="activity"></param>
        /// <param name="sourceTempPath"></param>
        /// <param name="tempPath"></param>
        /// <param name="indivFileSize"></param>
        /// <param name="transferTime"></param>
        public void CreateDailyLogFile(string SaveName, string activity, string sourceTempPath, string tempPath, long indivFileSize, double transferTime)
        {
            ReadJsonSave();

            Dailylogfile.DailyLog.Add(new DailyLogsJson()
            {
                Name = SaveName,
                Timestamp = DateTime.Now.ToString("HH:mm:ss tt - dd/MM"),
                Activity = activity,
                Source = sourceTempPath,
                Destination = tempPath,
                FileSize = indivFileSize,
                TransferTime = transferTime
            });
            _m.WaitOne();
            Data = JsonConvert.SerializeObject(Dailylogfile, Formatting.Indented);
            File.WriteAllText(dailylogPath, Data);
            _m.ReleaseMutex();
        }

        /// <summary>
        /// Initalizes the daily log list if the file doesn't exist.
        /// This method makes sure we don't overwrite existing data in the daily log files.
        /// </summary>
        public void ReadJsonSave()
        {
            _m.WaitOne();
            if (!File.Exists(dailylogPath))
            {
                Dailylogfile = new DailyLogList() { DailyLog = new List<DailyLogsJson>() };
                Data = JsonConvert.SerializeObject(Dailylogfile);
                File.WriteAllText(dailylogPath, Data);
            }

            Data = File.ReadAllText(dailylogPath);
            Dailylogfile = JsonConvert.DeserializeObject<DailyLogList>(Data);
            _m.ReleaseMutex();

        }

    }
}
