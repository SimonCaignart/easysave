using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;


namespace EasySave
{
    class SaveJson
    {
        public string Data;

        Mutex _m = new Mutex();

        readonly string appPath = Path.GetFullPath(@"Config\Save.json");

        public SaveList SavedFile { get; set; }

        //Constructor
        public SaveJson()
        {
            Data = "";
            SavedFile = new SaveList();
            Directory.CreateDirectory(Path.GetFullPath(@"Config"));
        }
        /// <summary>
        ///Put all the information about the saves on a JSON file.
        ///</summary> 
        /// <param name="label"> Name of one save.</param>
        /// <param name="source">Source of the save.</param>
        /// <param name="destination">Destination of the save.</param>
        /// <param name="saveType">Type of the save.</param>
        public void InsertSaveJson(string label, string source, string destination, string saveType)
        {

            ReadJsonSave();

            SavedFile.Save.Add(new SavesInfo() { Name = label, Source = source, Destination = destination, SaveType = saveType });

            Data = JsonConvert.SerializeObject(SavedFile, Formatting.Indented);
            File.WriteAllText(appPath, Data);

        }

        /// <summary>
        ///  Read and check if the JSON file exist. If it doesn't, create it.
        /// </summary>

        public void ReadJsonSave()
        {
            if (!File.Exists(appPath))
            {
                SavedFile = new SaveList() { Save = new List<SavesInfo>() };
                Data = JsonConvert.SerializeObject(SavedFile, Formatting.Indented);
                File.WriteAllText(appPath, Data);
            }

            _m.WaitOne();
            try
            {
                Data = File.ReadAllText(appPath);
                SavedFile = JsonConvert.DeserializeObject<SaveList>(Data);
            }
            catch (Exception e)
            {
                ReadJsonSave();
            }

            _m.ReleaseMutex();
        }

        /// <summary>
        /// List one save of the JSON.
        /// </summary>
        /// <param name="NumSave"> The number of the save to choose the correct one.</param>
        /// <returns>
        /// Read the information about one save.
        /// </returns>
        public SavesInfo ChooseOneSave(int NumSave)
        {
            Data = File.ReadAllText(appPath);

            SavedFile = JsonConvert.DeserializeObject<SaveList>(Data);

            return SavedFile.Save[NumSave];
        }

        /// <summary>
        /// List every saves on the JSON.
        /// </summary>
        /// <returns>
        /// An array with all of the saves.
        /// </returns>
        public string[] ListAllSaves()
        {
            int NumSave;
            string[] allSave;
            ReadJsonSave();

            for (NumSave = 0; NumSave < SavedFile.Save.Count; NumSave++)
            {

            }
            allSave = new string[NumSave];

            for (int i = 0; i < NumSave; i++)
            {
                allSave[i] = SavedFile.Save[i].Name + " (" + SavedFile.Save[i].SaveType + ")";
            }
            return allSave;
        }

        /// <summary>
        /// Delete the information of one save in the json.
        /// </summary>
        /// <param name="NumSave">The number of the save to delete the correct one.</param>
        public void DeleteSaveJson(string nameOfSave)
        {
            int nbSave = 0;
            ReadJsonSave();
            SavedFile = JsonConvert.DeserializeObject<SaveList>(Data);

            for (int i = 0; i < SavedFile.Save.Count; i++)
            {
                if (SavedFile.Save[i].Name == nameOfSave)

                {
                    nbSave = i;
                }
            }
            if (SavedFile.Save.Count != 0)
            {
                SavedFile.Save.Remove(SavedFile.Save[nbSave]);
            }

            Data = JsonConvert.SerializeObject(SavedFile, Formatting.Indented);
            File.WriteAllText(appPath, Data);

        }

        /// <summary>
        /// Save the information of one save to use it in the controller.
        /// </summary>
        /// <param name="number">The number of the save.</param>
        /// <returns> An array with all the informations of one save.</returns>
        public string[] SaveInformation(int number)
        {
            string[] SavedInformation = {
                SavedFile.Save[number].Name,
                SavedFile.Save[number].SaveType,
                SavedFile.Save[number].Source,
                SavedFile.Save[number].Destination
            };

            return SavedInformation;

        }
    }
}