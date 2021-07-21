using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Resources;
using EasySave.Helper_Classes;
using EasySave.Languages;
using System.Windows;
using System.Threading;
using EasySave.ViewModel;

namespace EasySave
{
    /// <summary>
    /// This class is the save model of easysave.
    /// Contains alls methods for performing operations on a save object.
    /// </summary>
    class Save : ObservableObject
    {



        Mutex _m = new Mutex();
        // Objects
        public SaveJson savejson;
        public ConfigFile configjson;
        public StateLog state;
        public DailyLog daily;

        // Save variables
        public string saveName;
        public string saveType;
        public string sourcePath;
        public string destinationPath;

        public string SaveName
        {
            get { return saveName; }
            set { saveName = value; OnPropertyChanged("SaveName"); }
        }

        public string SaveType
        {
            get { return saveType; }
            set { saveType = value; OnPropertyChanged("SaveType"); }
        }

        public string SrcPath
        {
            get { return sourcePath; }
            set { sourcePath = value; OnPropertyChanged("SrcPath"); }
        }

        public string DestPath
        {
            get { return destinationPath; }
            set { destinationPath = value; OnPropertyChanged("DestPath"); }
        }


        //ConfigFile variables
        string cryptoKey;
        string cryptedExtensions;
        public string CryptoSoftPath = Path.GetFullPath(@"CryptoSoft.exe");
        public List<Object> installedSoftware;
        WorkSoftware workSoftware = new WorkSoftware();

        public string CryptoKey
        {
            get { return cryptoKey; }
            set { cryptoKey = value; OnPropertyChanged("CryptoKey"); }
        }

        public string CryptedExtensions
        {
            get { return cryptedExtensions; }
            set { cryptedExtensions = value; OnPropertyChanged("CryptedExtensions"); }
        }

        public List<Object> InstalledSoftware
        {
            get { return installedSoftware; }
            set { installedSoftware = value; OnPropertyChanged("InstalledSoftware"); }
        }

        public string[] getProcessList()
        {
            return workSoftware.ProcessList();
        }

        /// <summary>
        /// Consrtuctor of save.
        /// </summary>
        /// <remarks>
        /// Instantiate SaveJson, StateLog and DailyLog.
        /// </remarks>
        //Constructor
        public Save()
        {
            savejson = new SaveJson();
            state = new StateLog();
            daily = new DailyLog();
            configjson = new ConfigFile();
        }

        public bool WorkSoftwareRunning()
        {
            var worksoftwarelist = configjson.GetBannedProcesses();
            var runningProcesses = getProcessList();
            bool processRunning = false;
            foreach (string worksoftware in worksoftwarelist)
            {
                foreach (string runningProcess in runningProcesses)
                {
                    if (worksoftware == runningProcess)
                    {
                        processRunning = true;
                    }
                }
            }
            return processRunning;
        }

        /// <summary>
        /// Run a complete save.
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        /// <remarks>
        /// This method also calls the statelog file creation and update.
        /// </remarks>
        /// <returns>
        /// A message indicating a success or an error.
        /// </returns>
        public void CompleteSave(string saveName,string SourcePath, string DestinationPath)
        {
            
            string progression;

            // Array containing the extensions needed to be crypted
            string[] cryptedExtensionsArray = GetConfigFileElement("CryptedExtensions").Split(',');

            // Array containing the priority extensions
            string[] priorityExtensionsArray = GetConfigFileElement("PriorityExtensions").Split(',');

            // Max File Size that can be copied, converted to bytes
            long maxFileSize = long.Parse(GetConfigFileElement("FileMaxSize")) * 1000000;

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(SourcePath);

            int numberOfFiles = Directory.GetFiles(SourcePath, "*.*", SearchOption.TopDirectoryOnly).Length;

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exists, create it, if it exists does nothing      
            Directory.CreateDirectory(DestinationPath);
            // Gets directory info to enumerate files for logs
            DirectoryInfo dirdest = new DirectoryInfo(DestinationPath);

            // Get the files in the source directory.
            FileInfo[] files = dir.GetFiles();
            FileInfo[] filesSortedByPriority = new FileInfo[files.Length];

            FileInfo emptyFileInfo = new FileInfo(@"C:\NonExistant.txt");
            Array.Fill(filesSortedByPriority, emptyFileInfo);

            int nbrFilesSorted = 0;

            long fileSize = dir.EnumerateFiles().Sum(files => files.Length);

            if (priorityExtensionsArray[0] != "")
            {
                // Add all the priority files in the sorted array
                foreach (FileInfo file in files)
                {
                    foreach (string priorityExtension in priorityExtensionsArray)
                    {
                        if (priorityExtension == file.Extension)
                        {
                            filesSortedByPriority[nbrFilesSorted] = file;
                            nbrFilesSorted++;
                        }
                    }
                }

                // Add the remaining files
                bool AlreadyInSortedArray = false;
                foreach (FileInfo file in files)
                {
                    AlreadyInSortedArray = false;

                    foreach (FileInfo filesorted in filesSortedByPriority)
                    {
                        if (file.FullName == filesorted.FullName)
                        {
                            AlreadyInSortedArray = true;
                        }
                    }
                    if (!AlreadyInSortedArray)
                    {
                        filesSortedByPriority[nbrFilesSorted] = file;
                        nbrFilesSorted++;
                    }
                }
            }
            else
            {
                filesSortedByPriority = files;
            }

            // Initialise the file transfer count
            int filesDone = 0;
            bool isStopped = false;
            // Copy each file to the new directory.
            foreach (FileInfo file in filesSortedByPriority)
            {

                while (state.ReadJsonBool(saveName) == 2 || state.ReadJsonBool(saveName) == 3 || WorkSoftwareRunning())
                {
                    Thread.Sleep(500);

                    if (state.ReadJsonBool(saveName) == 3)
                    {
                        isStopped = true;
                        break;
                    }
                }
                if (isStopped || state.ReadJsonBool(saveName) == 3)
                {
                    break;
                }
                    // Check if the file need to be crypted
                    bool CryptFile = false;

                foreach (string extension in cryptedExtensionsArray)
                {
                    if (extension == file.Extension)
                    {
                        CryptFile = true;
                    }
                }

                // Copy the file only if its size is less than the maxFileSize
                if (file.Length <= maxFileSize || maxFileSize == 0)
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    filesDone++;

                    string tempPath = Path.Combine(DestinationPath, file.Name);
                    string sourceTempPath = Path.Combine(SourcePath, file.Name);

                    string encryptionTime = "Not Crypted";
                    switch (CryptFile)
                    {
                        case true:
                            _m.WaitOne();
                            encryptionTime = CryptAFile(sourceTempPath, tempPath);
                            _m.ReleaseMutex();

                            break;

                        case false:
                            _m.WaitOne();
                            file.CopyTo(tempPath, true);
                            _m.ReleaseMutex();
                            break;
                    }

                    long indivFileSize = file.Length;
                    long destinationCurrentSize = dirdest.EnumerateFiles().Sum(files => files.Length);
                    int filesRemaining = (numberOfFiles - filesDone);

                    if (destinationCurrentSize == 0 || fileSize == 0 || filesRemaining == 0)
                    {
                        progression = "100";
                    }
                    else
                    {
                        progression = (100 - (100 * filesRemaining / numberOfFiles)).ToString();
                    }

                    string fileSizeRemaining = ((double)fileSize - destinationCurrentSize).ToString();

                    string activeornot = "";
                    string activity = "Run a complete save!";
                    if (filesRemaining == 0)
                    {
                        activeornot = "No. Save ended";
                    }
                    else
                    {
                        activeornot = "Yes, currently saving";
                    }

                    timer.Stop();
                    double transferTime = timer.Elapsed.TotalMilliseconds;
                    _m.WaitOne();
                    state.CreateStateLogFile(saveName, SaveType, activity, activeornot, sourceTempPath, tempPath, numberOfFiles, fileSize, progression, fileSizeRemaining, filesRemaining, encryptionTime);
                    daily.CreateDailyLogFile(saveName, activity, sourceTempPath, tempPath, indivFileSize, transferTime);
                    _m.ReleaseMutex();
                    Thread.Sleep(70);
                }
            }

            // Run the save for each subdirectory in the source directory
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(DestinationPath, subdir.Name);
                CompleteSave(saveName,subdir.FullName, tempPath);
            }
        }

        /// <summary>
        /// Run a differential save.
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        /// <remarks>
        /// This method also calls the statelog file creation and update.
        /// </remarks>
        /// <returns>
        /// A message indicating a success or an error.
        /// </returns>
        public void DifferentialSave(string SourcePath, string DestinationPath)
        {
            string progression;

            // Array containing the extensions needed to be crypted
            string[] cryptedExtensionsArray = GetConfigFileElement("CryptedExtensions").Split(',');

            // Array containing the priority extensions
            string[] priorityExtensionsArray = GetConfigFileElement("PriorityExtensions").Split(',');

            // Max File Size that can be copied, converted to bytes
            long maxFileSize = long.Parse(GetConfigFileElement("FileMaxSize")) * 1000000;

            // Get the subdirectories for the source directory.
            DirectoryInfo sourcedir = new DirectoryInfo(SourcePath);

            // Get the subdirectories for the destination directory.
            DirectoryInfo destinationdir = new DirectoryInfo(DestinationPath);

            DirectoryInfo[] dirs = sourcedir.GetDirectories();

            // If the destination directory doesn't exists, create it, if it exists does nothing      
            Directory.CreateDirectory(DestinationPath);

            // Get the files in the source directory 
            FileInfo[] sourcefiles = sourcedir.GetFiles();

            // Array for the sorted files
            FileInfo[] filesSortedByPriority = new FileInfo[sourcefiles.Length];
            FileInfo emptyFileInfo = new FileInfo(@"C:\NonExistant.txt");

            // Fills the array with empty fileinfo
            Array.Fill(filesSortedByPriority, emptyFileInfo);

            // Get the files in the destination directory 
            FileInfo[] destinationfiles = destinationdir.GetFiles();


            long fileSize = sourcedir.EnumerateFiles().Sum(sourcefiles => sourcefiles.Length);

            // Count amount of files to move
            int numberOfFiles = Directory.GetFiles(SourcePath, "*.*", SearchOption.TopDirectoryOnly).Length;

            int nbrFilesSorted = 0;

            if (priorityExtensionsArray[0] != "")
            {
                // Add all the priority files in the sorted array
                foreach (FileInfo file in sourcefiles)
                {
                    foreach (string priorityExtension in priorityExtensionsArray)
                    {
                        if (priorityExtension == file.Extension)
                        {
                            filesSortedByPriority[nbrFilesSorted] = file;
                            nbrFilesSorted++;
                        }
                    }
                }

                // Add the remaining files
                bool AlreadyInSortedArray = false;
                foreach (FileInfo file in sourcefiles)
                {
                    AlreadyInSortedArray = false;

                    foreach (FileInfo filesorted in filesSortedByPriority)
                    {
                        if (file.FullName == filesorted.FullName)
                        {
                            AlreadyInSortedArray = true;
                        }
                    }
                    if (!AlreadyInSortedArray)
                    {
                        filesSortedByPriority[nbrFilesSorted] = file;
                        nbrFilesSorted++;
                    }
                }
            }
            else
            {
                filesSortedByPriority = sourcefiles;
            }

            // Initialise the file transfer count
            int filesDone = 0;

            // Copy each file to the new directory if it does not exist in the destination directory, 
            // or if the file in the destination directory is different from the source one
            foreach (FileInfo sourcefile in filesSortedByPriority)
            {

                // Indicate if the file need to be copied or not
                bool CopyFile = true;

                // Check if the file need to be crypted
                bool CryptFile = false;

                foreach (string extension in cryptedExtensionsArray)
                {
                    if (extension == sourcefile.Extension)
                    {
                        CryptFile = true;
                    }
                }

                // If there is an existing file in the dest directory, compare it with the source one, 
                // and if they are the same, the file will not be copied
                foreach (FileInfo destinationfile in destinationfiles)
                {
                    if ((sourcefile.LastWriteTime == destinationfile.LastWriteTime) && CryptFile == false)
                    {
                        CopyFile = false;
                    }
                }

                if (CopyFile == true && sourcefile.Length <= maxFileSize || maxFileSize == 0)
                {
                    // Source path and destination path of the file
                    string sourceTempPath = Path.Combine(SourcePath, sourcefile.Name);
                    string tempPath = Path.Combine(DestinationPath, sourcefile.Name);

                    string encryptionTime = "Not Crypted";

                    // Copy or crypt the file in the destination folder
                    switch (CryptFile)
                    {
                        case true:
                            _m.WaitOne();
                            encryptionTime = CryptAFile(sourceTempPath, tempPath);
                            _m.ReleaseMutex();
                            break;

                        case false:
                            _m.WaitOne();
                            sourcefile.CopyTo(tempPath, true);
                            _m.ReleaseMutex();
                            break;
                    }

                    var timer = new Stopwatch();
                    timer.Start();
                    filesDone++;
                    long indivFileSize = sourcefile.Length;
                    long destinationCurrentSize = destinationdir.EnumerateFiles().Sum(destinationfile => destinationfile.Length);
                    if (destinationCurrentSize == 0 || fileSize == 0)
                    {
                        progression = "100";
                    }
                    else
                    {
                        progression = (100 * destinationCurrentSize / fileSize).ToString();
                    }
                    string fileSizeRemaining = ((double)fileSize - destinationCurrentSize).ToString();
                    int filesRemaining = (numberOfFiles - filesDone);
                    string activeornot;
                    string activity = "Run a differential save!";
                    if (filesRemaining == 0)
                    {
                        activeornot = "No. Save ended";
                    }
                    else
                    {
                        activeornot = "Yes, currently saving";
                    }
                    timer.Stop();
                    double transferTime = timer.Elapsed.TotalMilliseconds;
                    state.CreateStateLogFile(SaveName, SaveType, activity, activeornot, sourceTempPath, tempPath, numberOfFiles, fileSize, progression, fileSizeRemaining, filesRemaining, encryptionTime);
                    daily.CreateDailyLogFile(SaveName, activity, sourceTempPath, tempPath, indivFileSize, transferTime);
                }
            }

            // Run the save for each subdirectory in the source directory
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(DestinationPath, subdir.Name);
                DifferentialSave(subdir.FullName, tempPath);
            }
        }

        /// <summary>
        /// Delete a save in the json, and if param destruct save is true, delete the save destination directory.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="destructSave"></param>
        /// <returns>
        /// A message indicating a success or an error.
        /// </returns>
        public void DeleteSave(string nameOfSave)
        {
            savejson.DeleteSaveJson(nameOfSave);
            state.DeleteStateJson(nameOfSave);
        }

        /// <summary>
        /// Insert saves informations in the json save file.
        /// </summary>
        public void InsertIntoJson(string name,string sourcePath, string destinationPath)
        {
            savejson.InsertSaveJson(name, sourcePath, destinationPath, SaveType);
        }

        /// <summary>
        /// Used to get all the saves from the json file
        /// </summary>
        /// <returns></returns>
        public string[] getAllSaves()
        {
            return savejson.ListAllSaves();
        }

        public string[] getStateLogs()
        {
            return state.ListStateLogs();
        }


        /// <summary>
        /// Launch CryptoSoft.exe with the good command line arguments
        /// </summary>
        /// <param name="SourceFilePath"></param>
        /// <param name="DestinationFilePath"></param>
        /// <returns> Return an error if the result is less than zero, if it is more than 0, the returned value is the encryption time of the file ></returns>
        public string CryptAFile(string SourceFilePath, string DestinationFilePath)
        {
            try
            {
                // Command Line Arguments
                string commandline = "source \"" + SourceFilePath + "\" destination \"" + DestinationFilePath + "\"";

                Process cryptosoft = new Process();
                cryptosoft.StartInfo.UseShellExecute = false;
                cryptosoft.StartInfo.Arguments = commandline;
                cryptosoft.StartInfo.FileName = CryptoSoftPath;
                cryptosoft.StartInfo.CreateNoWindow = true;
                cryptosoft.Start();
                cryptosoft.WaitForExit();
                return cryptosoft.ExitCode.ToString();
            }
            catch (Exception)
            {
                return "-1";
            }
        }


        /// <summary>
        /// Create And/Or Update the configFile json
        /// </summary>
        /// <param name="cryptoKey"></param>
        /// <param name="cryptedExtensions"></param>
        public void UpdateConfigFile(string language, string fileMaxSize, string cryptoKey, string cryptedExtensions, string workSoftware, string priorityExtensions)
        {
            configjson.UpdateConfigFile(language, fileMaxSize, cryptoKey, cryptedExtensions, workSoftware, priorityExtensions);
        }


        /// <summary>
        /// Used to get a specific element from the config json file
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string GetConfigFileElement(string element)
        {
            return configjson.getConfigFileElement(element);
        }

        /// <summary>
        /// Used to remove a crypted extension from the configjson file
        /// </summary>
        /// <param name="selectedCryptedExtension"></param>
        public void RemoveCryptedExtension(string selectedCryptedExtension)
        {
            configjson.DeleteCryptedExtension(selectedCryptedExtension);
        }

        /// <summary>
        /// Used to remove a priority extension from the configjson file
        /// </summary>
        /// <param name="selectedPriorityExtension"></param>
        public void RemovePriorityExtension(string selectedPriorityExtension)
        {
            configjson.DeletePriorityExtension(selectedPriorityExtension);
        }

        /// <summary>
        /// Used to remove an installed software from the configjson file
        /// </summary>
        /// <param name="selectedInstalledSoftware"></param>
        public void RemoveInstalledSoftware(string selectedInstalledSoftware)
        {
            var currentworksoftware = GetConfigFileElement("WorkSoftware").Split(',');
            bool exists = false;
            foreach (string worksoftware in currentworksoftware)
            {
                if (selectedInstalledSoftware == worksoftware)
                {
                    exists = true;
                }
            }
            if (exists != true)
            {
                configjson.DeleteInstalledSoftware(selectedInstalledSoftware);
            }
        }

        /// <summary>
        /// Used to add a banned installed software from the config file
        /// </summary>
        /// <param name="SelectedDeletedInstalledSoftware"></param>
        public void AddInstalledSoftware(string SelectedDeletedInstalledSoftware)
        {
            try
            {
                configjson.AllowInstalledSoftware(SelectedDeletedInstalledSoftware);
            }
            catch (NullReferenceException)
            {
                //do nothing
            }

        }


    }
}