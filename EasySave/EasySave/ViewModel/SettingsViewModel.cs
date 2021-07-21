using EasySave.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Input;


namespace EasySave.ViewModel
{
    class SettingsViewModel : ObservableObject
    {
        private MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
        public ResourceManager rm;
        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsViewModel()
        {
            this.rm = mainWindow.RM;
        }

        private readonly Save save = new Save();

        // MaxFileSize 
        string fileMaxSize;

        public string FileMaxSizeInput
        {
            get { return save.GetConfigFileElement("FileMaxSize"); }
            set { fileMaxSize = value; OnPropertyChanged("FileMaxSizeInput"); }
        }

        /// <summary>
        /// Call the command when you click on the button "modify"
        /// </summary>
        private ICommand modifyFileMaxSizeCommand;

        public ICommand ModifyFileMaxSizeCommand
        {
            get
            {
                if (modifyFileMaxSizeCommand == null)
                {
                    modifyFileMaxSizeCommand = new RelayCommand(
                        param => EditFileMaxSize()
                        );
                }
                return modifyFileMaxSizeCommand;
            }
        }

        /// <summary>
        /// Edit the json file with the maxfilesize input
        /// </summary>
        public void EditFileMaxSize()
        {
            if (fileMaxSize == "" || fileMaxSize =="0")
            {
                save.UpdateConfigFile("","0", "", "", "","");
            }
            if (fileMaxSize.All(char.IsDigit))
            {        
                save.UpdateConfigFile("",fileMaxSize, "", "", "","");
            }
            else
            {
                MessageBox.Show(rm.GetString("notValidSize"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            FileMaxSizeInput = save.GetConfigFileElement("FileMaxSize");
        }

        //CryptoKey variable
        string cryptoKeyInput;

        public string CryptoKeyInput
        {
            get { return save.GetConfigFileElement("CryptoKey"); }
            set { cryptoKeyInput = value; OnPropertyChanged("CryptoKeyInput"); }
        }

        /// <summary>
        /// Call the command when you click on the button "modify"
        /// </summary>
        private ICommand modifyCryptoKeyCommand;

        public ICommand ModifyCryptoKeyCommand
        {
            get
            {
                if (modifyCryptoKeyCommand == null)
                {
                    modifyCryptoKeyCommand = new RelayCommand(
                        param => EditCryptoKey()
                        );
                }
                return modifyCryptoKeyCommand;
            }
        }

        /// <summary>
        ///  Edit the json file with the new crypto key
        /// </summary>
        public void EditCryptoKey()
        {
            if (!string.IsNullOrEmpty(cryptoKeyInput))
            {
                byte[] keyInBytes = Encoding.ASCII.GetBytes(cryptoKeyInput);

                if (keyInBytes.Length < 64)
                {
                    MessageBox.Show(rm.GetString("keyPart1") + keyInBytes.Length + rm.GetString("keyPart2"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    save.UpdateConfigFile("","", String.Concat(cryptoKeyInput.Where(c => !Char.IsWhiteSpace(c))),"", "","");
                }
                CryptoKeyInput = save.GetConfigFileElement("CryptoKey");
            }
        }

        // Crypted Extensions variables
        string[] cryptedExtensions;
        string cryptedExtensionInput ="";
        string selectedCryptedExtension;

        /// <summary>
        /// Edit the json config file with the new cryptedextensions
        /// </summary>
        public string[] CryptedExtensions
        {
            get
            {
                if (string.IsNullOrEmpty(save.GetConfigFileElement("CryptedExtensions")))
                {
                    return null;
                }
                else
                {
                    return save.GetConfigFileElement("CryptedExtensions").Split(',');
                };
            }
            set { cryptedExtensions = value; OnPropertyChanged("CryptedExtensions"); }
        }

        public string CryptedExtensionInput
        {
            get { return cryptedExtensionInput; }
            set { cryptedExtensionInput = value; OnPropertyChanged("CryptedExtensionInput"); }
        }
        public string SelectedCryptedExtension
        {
            get { return selectedCryptedExtension; }
            set { selectedCryptedExtension = value; OnPropertyChanged("SelectedCryptedExtension"); }
        }

        /// <summary>
        /// Call the command when you click on the button "save"
        /// </summary>
        private ICommand addCryptedExtensionsCommand;

        public ICommand AddCryptedExtensionsCommand
        {
            get
            {
                if (addCryptedExtensionsCommand == null)
                {
                    addCryptedExtensionsCommand = new RelayCommand(
                        param => AddCryptedExtensions()
                        );
                }
                return addCryptedExtensionsCommand;
            }
        }


        /// <summary>
        /// Method to update the config file
        /// </summary>
        public void AddCryptedExtensions()
        {
            // Get the number of '.' in the cryptedExtensionInput
            int numberOfDots = cryptedExtensionInput.Count(f => (f == '.'));

            if ( cryptedExtensionInput == null || cryptedExtensionInput == "")
            {
                // Does nothing
            }
            else if (save.GetConfigFileElement("CryptedExtensions").Contains(cryptedExtensionInput))
            {
                MessageBox.Show(rm.GetString("extensionAlreadyExist"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (cryptedExtensionInput[0] == '.' && numberOfDots == 1 && cryptedExtensionInput.Length > 1 && !cryptedExtensionInput.Contains(','))
            {
                save.UpdateConfigFile("","", "", String.Concat(cryptedExtensionInput.Where(c => !Char.IsWhiteSpace(c))), "","");
            }
            else
            {
                MessageBox.Show(rm.GetString("extensionNotValid"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            CryptedExtensions = save.GetConfigFileElement("CryptedExtensions").Split(',');
        }

        /// <summary>
        /// Call the command when you click on the button "delete"
        /// </summary>
        private ICommand deleteCryptedExtensionsCommand;

        public ICommand DeleteCryptedExtensionsCommand
        {
            get
            {
                if (deleteCryptedExtensionsCommand == null)
                {
                    deleteCryptedExtensionsCommand = new RelayCommand(
                        param => DeleteCryptedExtensions()
                        );
                }
                return deleteCryptedExtensionsCommand;
            }
        }

        /// <summary>
        /// Method to update the config file
        /// </summary>
        public void DeleteCryptedExtensions()
        {
            if (selectedCryptedExtension != null)
            {
            save.RemoveCryptedExtension(selectedCryptedExtension);
            CryptedExtensions = save.GetConfigFileElement("CryptedExtensions").Split(',');
            }
        }


        // Priority Extensions variables
        string[] priorityExtensions;
        string priorityExtensionInput = "";
        string selectedPriorityExtension;

        /// <summary>
        /// Edit the json config file with the new cryptedextensions
        /// </summary>
        public string[] PriorityExtensions
        {
            get
            {
                if (string.IsNullOrEmpty(save.GetConfigFileElement("PriorityExtensions")))
                {
                    return null;
                }
                else
                {
                    return save.GetConfigFileElement("PriorityExtensions").Split(',');
                };
            }
            set { cryptedExtensions = value; OnPropertyChanged("PriorityExtensions"); }
        }

        public string PriorityExtensionInput
        {
            get { return priorityExtensionInput; }
            set { priorityExtensionInput = value; OnPropertyChanged("PriorityExtensionInput"); }
        }
        public string SelectedPriorityExtension
        {
            get { return selectedPriorityExtension; }
            set { selectedPriorityExtension = value; OnPropertyChanged("SelectedPriorityExtension"); }
        }

        /// <summary>
        /// Call the command when you click on the button "save"
        /// </summary>
        private ICommand addPriorityExtensionsCommand;

        public ICommand AddPriorityExtensionsCommand
        {
            get
            {
                if (addPriorityExtensionsCommand == null)
                {
                    addPriorityExtensionsCommand = new RelayCommand(
                        param => AddPriorityExtensions()
                        );
                }
                return addPriorityExtensionsCommand;
            }
        }

        /// <summary>
        /// Method to update the config file
        /// </summary>
        public void AddPriorityExtensions()
        {
            // Get the number of '.' in the cryptedExtensionInput
            int numberOfDots = priorityExtensionInput.Count(f => (f == '.'));

            if (priorityExtensionInput == null || priorityExtensionInput == "")
            {
                // Does nothing
            }
            else if (save.GetConfigFileElement("PriorityExtensions").Contains(priorityExtensionInput))
            {
                MessageBox.Show(rm.GetString("extensionAlreadyExist"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (priorityExtensionInput[0] == '.' && numberOfDots == 1 && priorityExtensionInput.Length > 1 && !priorityExtensionInput.Contains(','))
            {
                save.UpdateConfigFile("", "", "", "", "", String.Concat(priorityExtensionInput.Where(c => !Char.IsWhiteSpace(c))));
            }
            else
            {
                MessageBox.Show(rm.GetString("extensionNotValid"), "Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            PriorityExtensions = save.GetConfigFileElement("PriorityExtensions").Split(',');
        }

        /// <summary>
        /// Call the command when you click on the button "delete"
        /// </summary>
        private ICommand deletePriorityExtensionsCommand;

        public ICommand DeletePriorityExtensionsCommand
        {
            get
            {
                if (deletePriorityExtensionsCommand == null)
                {
                    deletePriorityExtensionsCommand = new RelayCommand(
                        param => DeletePriorityExtensions()
                        );
                }
                return deletePriorityExtensionsCommand;
            }
        }

        /// <summary>
        /// Method to update the config file
        /// </summary>
        public void DeletePriorityExtensions()
        {
            if (selectedPriorityExtension != null)
            {
                save.RemovePriorityExtension(selectedPriorityExtension);
                PriorityExtensions = save.GetConfigFileElement("PriorityExtensions").Split(',');
            }
        }

        string[] installedSoftware;
        
        public string[] InstalledSoftware
        {
            get { return save.getProcessList(); }
            set { installedSoftware = value; OnPropertyChanged("InstalledSoftware"); }
        }

        string selectedInstalledSoftware;

        public string SelectedInstalledSoftware
        {
            get { return selectedInstalledSoftware; }
            set { selectedInstalledSoftware = value; OnPropertyChanged("SelectedInstalledSoftware"); }
        }

        string selectedDeletedInstalledSoftware;

        public string SelectedDeletedInstalledSoftware
        {
            get { return selectedDeletedInstalledSoftware; }
            set { selectedDeletedInstalledSoftware = value; OnPropertyChanged("SelectedDeletedInstalledSoftware"); }
        }

        string[] deletedInstalledSoftware;

        /// <summary>
        /// Array of all the software marked as "work software" - if this software is running you can't save.
        /// </summary>
        public string[] DeletedInstalledSoftware
        {
            get
            {
                if (string.IsNullOrEmpty(save.GetConfigFileElement("WorkSoftware")))
                {
                    return null;
                }
                else
                {
                    return save.GetConfigFileElement("WorkSoftware").Split(',');
                };
            }

            set { deletedInstalledSoftware = value; OnPropertyChanged("DeletedInstalledSoftware"); }
        }

        private ICommand addSelectedInstalledSoftware;

        /// <summary>
        /// Command for the button that unmarks software as being "work software".
        /// </summary>
        public ICommand AddSelectedInstalledSoftware
        {
            get
            {
                if (addSelectedInstalledSoftware == null)
                {
                    addSelectedInstalledSoftware = new RelayCommand(
                        param => AddInstalledSoftware()
                        );
                }
                return addSelectedInstalledSoftware;
            }
        }

        /// <summary>
        /// Method to update JSON & view.
        /// </summary>
        public void AddInstalledSoftware()
        {
            try
            {
                if(SelectedDeletedInstalledSoftware != null)
                {
                save.AddInstalledSoftware(SelectedDeletedInstalledSoftware);
                DeletedInstalledSoftware = save.GetConfigFileElement("WorkSoftware").Split(',');
                }
            }
            catch(Exception)
            {
                //do nothing
            }
        }

        private ICommand deleteSelectedInstalledSoftware;

        /// <summary>
        /// Command for the button that marks software as being "work software".
        /// </summary>
        public ICommand DeleteSelectedInstalledSoftware
        {
            get
            {
                if (deleteSelectedInstalledSoftware == null)
                {
                    deleteSelectedInstalledSoftware = new RelayCommand(
                        param => DeleteInstalledSoftware()
                        );
                }
                return deleteSelectedInstalledSoftware;
            }
        }

        /// <summary>
        /// Method to update JSON & view.
        /// </summary>
        public void DeleteInstalledSoftware()
        {
            save.RemoveInstalledSoftware(selectedInstalledSoftware);
            DeletedInstalledSoftware = save.GetConfigFileElement("workSoftware").Split(',');
        }
    }
}