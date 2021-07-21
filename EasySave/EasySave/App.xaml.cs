using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Checks if the app is already running, if it is, show a message and close the app
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1)
            {
                MessageBox.Show("Another instance of this program is already running. Cannot proceed further. \n Une autre instance de ce programme est en cours d'execution. Impossible de continuer", "Warning !");
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                // Launh the server for the EasySave deported console
                Server server = new Server();
               
                this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
     
            MainWindow mainWindow = new MainWindow();

        }
    }
}
