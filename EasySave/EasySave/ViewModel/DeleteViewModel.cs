using EasySave.Helper_Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace EasySave.ViewModel
{
    class DeleteViewModel : ObservableObject
    {
        Save save = new Save();
        public string[] listSave;
        public string selectedSave;

        private MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
        public ResourceManager rm;

        public DeleteViewModel()
        {
            this.rm = mainWindow.RM;
        }

        public string[] ListSave
        {
            get { return save.getAllSaves(); }
            set { listSave = value; OnPropertyChanged("ListSave"); }
        }

        public string SelectedSave
        {
            get { return selectedSave; }
            set { selectedSave = value; OnPropertyChanged("SelectedSave"); }
        }

        public ICommand deleteSave;

        public ICommand DeleteSave
        {
            get
            {
                if (deleteSave == null)
                {
                    deleteSave = new RelayCommand(
                        param => DeleteSaveJson()
                        );
                }
                return deleteSave;
            }
        }

        public void DeleteSaveJson()
        {
            if (selectedSave != "" && selectedSave != null)
            {
                save.DeleteSave((selectedSave.Split('(')[0]).Trim());
                ListSave = save.getAllSaves();
                MessageBox.Show(rm.GetString("deleteSave"), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
