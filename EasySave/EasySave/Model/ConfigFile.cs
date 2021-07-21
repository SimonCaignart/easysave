using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace EasySave
{
    class ConfigFile
    {

        public string Data;
        ConfigFileJson configFileJson;

        private static Mutex mut = new Mutex();

        public string configfilepath = Path.GetFullPath(@"Config\config.json");

        public ConfigFile()
        {
            Directory.CreateDirectory(Path.GetFullPath(@"Config"));

            // Generate config file if it does not exist, a cryptoKey of 64 bits is also created
            if (!File.Exists(configfilepath))
            {
                configFileJson = new ConfigFileJson
                {
                    language = "",
                    fileMaxSize = "2048",
                    cryptoKey = GenerateCryptoKey(),
                    cryptedExtensions = "",
                    priorityExtensions = "",
                    workSoftware = ""
                };

                Data = JsonConvert.SerializeObject(configFileJson);

                File.WriteAllText(configfilepath, Data);
            }
            bannedProcesses = GetBannedProcesses();
        }

        public string GenerateCryptoKey()
        {
            // Generate a random string of 64 bit 
            StringBuilder strbuilder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 64; i++)
            {
                // Generate floating point numbers
                double myFloat = random.NextDouble();
                // Generate the char using below logic
                var myChar = Convert.ToChar(Convert.ToInt32(Math.Floor(25 * myFloat) + 65));
                strbuilder.Append(myChar);
            }
            return strbuilder.ToString();
        }

        /// <summary>
        /// This method update the configfile json with the new values passed in parameters
        /// </summary>
        /// <param name="language"></param>
        /// <param name="fileMaxSize"></param>
        /// <param name="cryptoKey"></param>
        /// <param name="cryptedExtensions"></param>
        /// <param name="workSoftware"></param>
        /// <param name="priorityExtensions"></param>
        /// 
        public void UpdateConfigFile(string language, string fileMaxSize, string cryptoKey, string cryptedExtensions, string workSoftware, string priorityExtensions)
        {
            mut.WaitOne();
            Data = File.ReadAllText(configfilepath);
            configFileJson = JsonConvert.DeserializeObject<ConfigFileJson>(Data);
            mut.ReleaseMutex();

            if (language != "")
            {
                configFileJson.language = language;
            }

            if (fileMaxSize != "")
            {
                configFileJson.fileMaxSize = fileMaxSize;
            }

            if (cryptoKey != "")
            {
                configFileJson.cryptoKey = cryptoKey;
            }

            if (cryptedExtensions != "")
            {
                if (configFileJson.cryptedExtensions == null || configFileJson.cryptedExtensions == "")
                {
                    configFileJson.cryptedExtensions += cryptedExtensions;
                }
                else
                {
                    configFileJson.cryptedExtensions += "," + cryptedExtensions;
                }
            }

            if (workSoftware != "")
            {
                if (configFileJson.workSoftware == null || configFileJson.workSoftware == "")
                {
                    configFileJson.workSoftware += workSoftware;
                }
                else
                {
                    configFileJson.workSoftware += "," + workSoftware;
                }
            }

            if (priorityExtensions != "")
            {
                if (configFileJson.priorityExtensions == null || configFileJson.priorityExtensions == "")
                {
                    configFileJson.priorityExtensions += priorityExtensions;
                }
                else
                {
                    configFileJson.priorityExtensions += "," + priorityExtensions;
                }
            }

            mut.WaitOne();
            // Serialize the configFile object in json and write it in the file
            Data = JsonConvert.SerializeObject(configFileJson, Formatting.Indented);
            File.WriteAllText(configfilepath, Data);
            mut.ReleaseMutex();
        }

        /// <summary>
        /// This method return the value of the element in the json passed in param
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string getConfigFileElement(string element)
        {
            mut.WaitOne();
            Data = File.ReadAllText(configfilepath);

            configFileJson = JsonConvert.DeserializeObject<ConfigFileJson>(Data);
            mut.ReleaseMutex();

            switch (element)
            {
                case "FileMaxSize":
                    element = configFileJson.fileMaxSize;
                    break;
                case "CryptoKey":
                    element = configFileJson.cryptoKey;
                    break;
                case "CryptedExtensions":
                    element = configFileJson.cryptedExtensions;
                    break;
                case "WorkSoftware":
                    element = configFileJson.workSoftware;
                    break;
                case "PriorityExtensions":
                    element = configFileJson.priorityExtensions;
                    break;
            }

            return element;
        }

        /// <summary>
        /// Used to delete a crypted extension in the json file
        /// </summary>
        /// <param name="selectedCryptedExtension"></param>
        public void DeleteCryptedExtension(string selectedCryptedExtension)
        {
            mut.WaitOne();
            Data = File.ReadAllText(configfilepath);

            configFileJson = JsonConvert.DeserializeObject<ConfigFileJson>(Data);

            string[] extensions = configFileJson.cryptedExtensions.Split(',');

            string final = "";
            bool firstOne = true;

            foreach (string extension in extensions)
            {
                if (extension != selectedCryptedExtension && !string.IsNullOrEmpty(selectedCryptedExtension))
                {
                    if (firstOne == true)
                    {
                        final += extension;
                        firstOne = false;
                    }
                    else
                    {
                        final += "," + extension;
                    }
                }
            }

            configFileJson.cryptedExtensions = final;

            Data = JsonConvert.SerializeObject(configFileJson, Formatting.Indented);
            File.WriteAllText(configfilepath, Data);
            mut.ReleaseMutex();
        }

        /// <summary>
        /// Used to delete a crypted extension in the json file
        /// </summary>
        /// <param name="selectedCryptedExtension"></param>
        public void DeletePriorityExtension(string selectedPriorityExtension)
        {
            mut.WaitOne();
            Data = File.ReadAllText(configfilepath);

            configFileJson = JsonConvert.DeserializeObject<ConfigFileJson>(Data);

            string[] extensions = configFileJson.priorityExtensions.Split(',');

            string final = "";
            bool firstOne = true;

            foreach (string extension in extensions)
            {
                if (extension != selectedPriorityExtension && !string.IsNullOrEmpty(selectedPriorityExtension))
                {
                    if (firstOne == true)
                    {
                        final += extension;
                        firstOne = false;
                    }
                    else
                    {
                        final += "," + extension;
                    }
                }
            }

            configFileJson.priorityExtensions = final;

            Data = JsonConvert.SerializeObject(configFileJson, Formatting.Indented);
            File.WriteAllText(configfilepath, Data);
            mut.ReleaseMutex();
        }


        /// <summary>
        /// Used to delete a software in the json file
        /// </summary>
        /// <param name="stringselectedInstalledSoftware"></param>
        public void DeleteInstalledSoftware(string stringselectedInstalledSoftware)
        {
            UpdateConfigFile("","", "", "", stringselectedInstalledSoftware,"");
        }

        public string[] worksoftwarelist;

        /// <summary>
        /// Used to re allow a specific software
        /// </summary>
        /// <param name="stringselectedInstalledSoftware"></param>
        public void AllowInstalledSoftware(string stringselectedInstalledSoftware)
        {
            mut.WaitOne();
            Data = File.ReadAllText(configfilepath);
            configFileJson = JsonConvert.DeserializeObject<ConfigFileJson>(Data);

            worksoftwarelist = configFileJson.workSoftware.Split(',');

            string final = "";
            bool firstOne = true;

            foreach (string worksoftware in worksoftwarelist)
            {
                if (worksoftware != stringselectedInstalledSoftware && !string.IsNullOrEmpty(stringselectedInstalledSoftware))
                {
                    if (firstOne == true)
                    {
                        final += worksoftware;
                        firstOne = false;
                    }
                    else
                    {
                        final += "," + worksoftware;
                    }
                }
            }

            configFileJson.workSoftware = final;

            Data = JsonConvert.SerializeObject(configFileJson, Formatting.Indented);
            File.WriteAllText(configfilepath, Data);
            mut.ReleaseMutex();
        }

        public string[] bannedProcesses;

        /// <summary>
        /// USed to get banned processes from the json config file
        /// </summary>
        /// <returns></returns>
        public string[] GetBannedProcesses()
        {
            mut.WaitOne();
            Data = File.ReadAllText(configfilepath);
            configFileJson = JsonConvert.DeserializeObject<ConfigFileJson>(Data);

            bannedProcesses = configFileJson.workSoftware.Split(',');

            Data = JsonConvert.SerializeObject(configFileJson, Formatting.Indented);
            File.WriteAllText(configfilepath, Data);
            mut.ReleaseMutex();
            return bannedProcesses;
        }
    }
}
