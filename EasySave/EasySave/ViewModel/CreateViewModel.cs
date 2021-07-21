using EasySave.Helper_Classes;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace EasySave.ViewModel
{
    class CreateViewModel : ObservableObject
    {
        private readonly Save save = new Save();

        private static Mutex mut = new Mutex();

        private string saveType;
        private string saveName;
        private string srcPath;
        private string destPath;

        public string SaveType
        {
            get { return saveType; }
            set { saveType = value; OnPropertyChanged("SaveType"); }
        }

        public string SaveName
        {
            get { return saveName; }
            set { saveName = value; OnPropertyChanged("SaveName"); }
        }

        public string SrcPath
        {
            get { return srcPath; }
            set { srcPath = value; OnPropertyChanged("SrcPath"); }
        }

        public string DestPath
        {
            get { return destPath; }
            set { destPath = value; OnPropertyChanged("DestPath"); }
        }

        /// <summary>
        /// Call the command when you click on the button "save"
        /// </summary>
        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(
                        param => CreateSave()
                        );
                }
                return saveCommand;
            }
        }

        private MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
        public ResourceManager rm;
        /// <summary>
        /// Constructor of the ViewModel
        /// </summary>
        public CreateViewModel()
        {
            this.rm = mainWindow.RM;
        }

        /// <summary>
        /// Method to save a file
        /// </summary>
        public void CreateSave()
        {
            bool noSave = false;
            string[] tab = save.getAllSaves();

            var regex = new Regex(@"[^a-zA-Z0-9\s]");
            if (save.WorkSoftwareRunning())
            {
                MessageBox.Show(rm.GetString("software"));
            }
            else
            {
                for (int i = 0; i < tab.Length; i++)
                {
                    if (SaveName == tab[i].Split('(')[0].Trim())
                    {                        
                        MessageBox.Show(rm.GetString("sameName"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        noSave = true;
                    }
                }


                if (!string.IsNullOrEmpty(srcPath) && !string.IsNullOrEmpty(destPath))
                {
                    if (srcPath == destPath)
                    {
                        MessageBox.Show(rm.GetString("differentPath"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        noSave = true;
                    }
                }

                if (!string.IsNullOrEmpty(srcPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(srcPath);
                    if (!dir.Exists)
                    {
                        MessageBox.Show(rm.GetString("sourceNotFound"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        noSave = true;
                    }
                }
                else
                {
                    MessageBox.Show(rm.GetString("sourceEmpty"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    noSave = true;
                }
                if (!string.IsNullOrEmpty(destPath))
                {
                    if (!regex.IsMatch(destPath))
                    {
                        MessageBox.Show(rm.GetString("destinationNotFound"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        noSave = true;
                    }
                }
                else
                {
                    MessageBox.Show(rm.GetString("destinationEmpty"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    noSave = true;
                }

                if (noSave == false)
                {
                    try
                    {
                        save.SaveType = SaveType.Split(':')[1].Trim();
                    }
                    catch
                    {
                        save.SaveType = "Complete";
                    }

                    if (save.SaveType == "Differential")
                    {
                        Thread thread = new Thread(() => save.DifferentialSave(srcPath, destPath));
                        thread.Start();
                        MessageBox.Show(rm.GetString("differentialDone"), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Thread thread = new Thread(new ThreadStart(ThreadLoop));
                        thread.Start();
                        MessageBox.Show(rm.GetString("completeDone"), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        public void ThreadLoop()
        {
            save.CompleteSave(saveName, srcPath, destPath);
            save.InsertIntoJson(saveName, srcPath, destPath);
        }
    }
}
