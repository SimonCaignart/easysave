using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace EasySave
{
    /// <summary>
    /// Main state log class.
    /// Contains all the methods required to create the state log file and add all the necessary information.
    /// </summary>
    class StateLog
    {
        public string Data;
        Mutex _m = new Mutex();
        public StateLogList logfile { get; set; }

        public int isPaused = 1;
        /// <summary>
        /// Creates the state log file in the folder created for it.
        /// </summary>
        public string statelogPath = Path.GetFullPath(@"Logs\statelog\stateLog.json");

        /// <summary>
        /// Calls the state log list and creates the state log folder.
        /// </summary>
        public StateLog()
        {
            Directory.CreateDirectory(Path.GetFullPath(@"Logs\statelog"));
            if (!File.Exists(statelogPath))
            {
                logfile = new StateLogList() { StateLog = new List<StateLogsJson>() };
                Data = JsonConvert.SerializeObject(logfile, Formatting.Indented);
                File.WriteAllText(statelogPath, Data);
            }
        }

        public bool saveExists = false;

        /// <summary>
        /// Gives a value to all the information required for the build of the state log file.
        /// </summary>
        /// <returns>
        /// A state log file.
        /// </returns>
        /// <param name="SaveName"></param>
        /// <param name="SaveType"></param>
        /// <param name="activity"></param>
        /// <param name="activeornot"></param>
        /// <param name="sourceTempPath"></param>
        /// <param name="tempPath"></param>
        /// <param name="numberOfFiles"></param>
        /// <param name="fileSize"></param>
        /// <param name="progression"></param>
        /// <param name="fileSizeRemaining"></param>
        /// <param name="filesRemaining"></param>
        /// <param name="encryptionTime"></param>
        public void CreateStateLogFile(string SaveName, string SaveType, string activity, string activeornot, string sourceTempPath, string tempPath, int? numberOfFiles, long fileSize, string progression, string fileSizeRemaining, int? filesRemaining, string encryptionTime)
        {

            foreach (string stateloggggg in ListStateLogs())
            {
                if(stateloggggg == SaveName)
                {
                    saveExists = true;
                }
            }

            if (saveExists)
            {
                _m.WaitOne();
                ReadJsonSave();
                int nbSave = 0;
                for (int i = 0; i < logfile.StateLog.Count; i++)
                {
                    if (logfile.StateLog[i].Name == SaveName)

                    {
                        nbSave = i;
                    }
                }
                
                if (logfile.StateLog.Count != 0)
                {
                    logfile.StateLog.Remove(logfile.StateLog[nbSave]);
                    Data = JsonConvert.SerializeObject(logfile, Formatting.Indented);
                    File.WriteAllText(statelogPath, Data);
                }
                logfile.StateLog.Add(new StateLogsJson()
                {
                    Name = SaveName,
                    Type = SaveType,
                    Activity = activity,
                    IsActivelySaving = activeornot,
                    Timestamp = DateTime.Now.ToString("HH:mm:ss tt - dd/MM"),
                    Source = sourceTempPath,
                    Destination = tempPath,
                    NumberOfFiles = numberOfFiles,
                    TotalFileSize = fileSize,
                    Progression = progression + "%",
                    FilesSizeRemaining = fileSizeRemaining + " bytes",
                    FilesRemaining = filesRemaining,
                    EncryptionTime = encryptionTime.ToString() + " ms",
                    IsPaused = isPaused
                });
                _m.ReleaseMutex();
            }
            else
            {
                logfile.StateLog.Add(new StateLogsJson()
                {
                    Name = SaveName,
                    Type = SaveType,
                    Activity = activity,
                    IsActivelySaving = activeornot,
                    Timestamp = DateTime.Now.ToString("HH:mm:ss tt - dd/MM"),
                    Source = sourceTempPath,
                    Destination = tempPath,
                    NumberOfFiles = numberOfFiles,
                    TotalFileSize = fileSize,
                    Progression = progression + "%",
                    FilesSizeRemaining = fileSizeRemaining + " bytes",
                    FilesRemaining = filesRemaining,
                    EncryptionTime = encryptionTime.ToString() + " ms",
                    IsPaused = isPaused
                });
            }
            
            _m.WaitOne();
            Data = JsonConvert.SerializeObject(logfile, Formatting.Indented);
            File.WriteAllText(statelogPath, Data);
            _m.ReleaseMutex();
        }

        public void InsertBoolJson(int test, string name)
        {
            ReadJsonSave();

            for (int i = 0; i < logfile.StateLog.Count; i++)
            {
                if (logfile.StateLog[i].Name == name)
                {
                    logfile.StateLog[i].IsPaused = test;
                    isPaused = test;
                }
            }
            _m.WaitOne();
            Data = JsonConvert.SerializeObject(logfile, Formatting.Indented);
            File.WriteAllText(statelogPath, Data);
            _m.ReleaseMutex();
        }

        public string[] ListStateLogs()
        {
            int saveNumber;
            ReadJsonSave();
            for (saveNumber = 0; saveNumber < logfile.StateLog.Count; saveNumber++)
            {

            }

            string[] statelogs = new string[saveNumber];

            for(int i = 0; i < saveNumber; i++)
            {
                statelogs[i] = logfile.StateLog[i].Name;
            }

            return statelogs;
        }

        /// <summary>
        /// Resets the state log list to rewrite the log file instead of adding entries.
        /// </summary>
        public void ReadJsonSave()
        {      
            if (!File.Exists(statelogPath))
            {
                logfile = new StateLogList() { StateLog = new List<StateLogsJson>() };
                Data = JsonConvert.SerializeObject(logfile);
                File.WriteAllText(statelogPath, Data);
            }

            _m.WaitOne();
            try
            {
                Data = File.ReadAllText(statelogPath);
                logfile = JsonConvert.DeserializeObject<StateLogList>(Data);

            } catch(Exception e)
            {
                ReadJsonSave();
            }
            
            _m.ReleaseMutex();
        }


        public int ReadJsonBool(string name)
        {
            int test = 0;
            _m.WaitOne();
            ReadJsonSave();
            for (int i = 0; i < logfile.StateLog.Count; i++)
            {
                if (logfile.StateLog[i].Name == name)
                {
                    test = logfile.StateLog[i].IsPaused;
                }
            }
            _m.ReleaseMutex();
            return test;
        }

        public void DeleteStateJson(string name)
        {
            int nbSave = 0;
            ReadJsonSave();

            for (int i = 0; i < logfile.StateLog.Count; i++)
            {
                if (logfile.StateLog[i].Name == name)

                {
                    nbSave = i;
                }
            }
            if (logfile.StateLog.Count != 0)
            {
                logfile.StateLog.Remove(logfile.StateLog[nbSave]);
            }
            _m.WaitOne();
            Data = JsonConvert.SerializeObject(logfile, Formatting.Indented);
            File.WriteAllText(statelogPath, Data);
            _m.ReleaseMutex();
        }

        public string[] GetAllProgresses()
        {
            _m.WaitOne();
            ReadJsonSave();

            int nbSave = logfile.StateLog.Count;
            string[] result = new string[nbSave];

            for (int i = 0; i < nbSave; i++)
            {
                if(logfile.StateLog[i].IsActivelySaving == "Yes, currently saving")
                {
                    result[i] = logfile.StateLog[i].Progression;
                }
                else
                {
                    result[i] = "Not currently saving.";
                }               
            }
            _m.ReleaseMutex();
            return result;

        }
    }
}
