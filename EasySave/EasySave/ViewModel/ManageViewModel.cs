using EasySave.Helper_Classes;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasySave.ViewModel
{
    class ManageViewModel : ObservableObject
    {
        Save save = new Save();
        public string[] listSave;
        public string selectedSave;
        public string saveName;
        public string destPath;
        public string srcPath;
        public string saveType;

        List<Thread> m_threads_list;

        public List<Thread> threads_list { get => m_threads_list; set => m_threads_list = value; }

        private MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
        public ResourceManager rm;

        public ManageViewModel()
        {
            this.rm = mainWindow.RM;
            m_threads_list = new List<Thread>();
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

        public ICommand playThread;

        public ICommand PlayThread
        {
            get
            {
                if (playThread == null)
                {
                    playThread = new RelayCommand(
                        param => PlayAThread()
                        );
                }
                return playThread;
            }
        }

        public void PlayAThread()
        {
            if (selectedSave != null)
            {
                for (int i = 0; i < threads_list.Count; i++)
                {
                    if (threads_list[i].Name == selectedSave.Split('(')[0].Trim())
                    {
                        save.state.InsertBoolJson(1, threads_list[i].Name);
                    }
                }
            }
        }

        public ICommand pauseThread;

        public ICommand PauseThread
        {
            get
            {
                if (pauseThread == null)
                {
                    pauseThread = new RelayCommand(
                        param => PauseAThread()
                        );
                }
                return pauseThread;
            }
        }

        public void PauseAThread()
        {
            if (selectedSave != null)
            {
                for (int i = 0; i < threads_list.Count; i++)
                {
                    if (threads_list[i].Name == selectedSave.Split('(')[0].Trim())
                    {
                        save.state.InsertBoolJson(2, threads_list[i].Name);
                    }
                }
            }
        }

        public ICommand stopThread;

        public ICommand StopThread
        {
            get
            {

                if (stopThread == null)
                {
                    stopThread = new RelayCommand(
                        param => StopAThread()
                        );
                }
                return stopThread;
            }
        }

        public void StopAThread()
        {
            if (selectedSave != null)
            {
                for (int i = 0; i < threads_list.Count; i++)
                {
                    if (threads_list[i].Name == selectedSave.Split('(')[0].Trim())
                    {
                        save.state.InsertBoolJson(3, threads_list[i].Name);
                    }
                }
            }
        }

        public ICommand runSelectedSave;

        public ICommand RunSelectedSave
        {
            get
            {
                if (runSelectedSave == null)
                {
                    runSelectedSave = new RelayCommand(
                        param => RunSelectedSaves()
                        );
                }
                return runSelectedSave;
            }
        }

        public ICommand runAllSave;

        public ICommand RunAllSave
        {
            get
            {
                if (runAllSave == null)
                {
                    runAllSave = new RelayCommand(
                        param => RunAllSaves()
                        );
                }
                return runAllSave;
            }
        }

        public void RunAllSaves()
        {
            for (int i = 0; i < ListSave.Length; i++)
            {
                string[] SaveInformation = save.savejson.SaveInformation(i);

                saveName = SaveInformation[0];
                saveType = SaveInformation[1];
                srcPath = SaveInformation[2];
                destPath = SaveInformation[3];
                save.state.InsertBoolJson(1, saveName);

                Thread thread = new Thread(new ThreadStart(ThreadLoop));
                thread.Start();
                thread.Name = saveName;
                threads_list.Add(thread);
                Thread.Sleep(60);
            }
            MessageBox.Show(rm.GetString("runSaveDone"), "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        /// <summary>
        /// The SpecificSave function.
        /// Save one save.
        /// </summary>
        public void RunSelectedSaves()
        {
            int final = -1;

            for (int i = 0; i < ListSave.Length; i++)
            {
                if (selectedSave == ListSave[i])
                {
                    final = i;
                }
            }

            if (final != -1)
            {
                string[] SaveInformation = save.savejson.SaveInformation(final);

                saveName = SaveInformation[0];
                saveType = SaveInformation[1];
                srcPath = SaveInformation[2];
                destPath = SaveInformation[3];
                save.state.InsertBoolJson(1, saveName);

                Thread thread = new Thread(new ThreadStart(ThreadLoop));
                thread.Start();
                thread.Name = saveName;
                threads_list.Add(thread);
                Thread.Sleep(60);
            }
            MessageBox.Show(rm.GetString("runSaveDone"), "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        public void ThreadLoop()
        {
            if (save.saveType == "differential")
            {
                save.DifferentialSave(srcPath, destPath);
            }
            else
            {
                save.CompleteSave(saveName, srcPath, destPath);
            }

        }
    }
}
