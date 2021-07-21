using EasySave.Languages;
using EasySave.View;
using System.Resources;
using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // resource files
        private ResourceManager rm;

        public ResourceManager RM
        {
            get { return rm; }
            set { rm = value; }
        }

        private bool btnCreateWasClicked = false;
        private bool btnRunWasClicked = false;
        private bool btnManageWasClicked = false;
        private bool btnSettingsWasClicked = false;

        /// <summary>
        /// Constructor
        /// Initialize the main window.
        /// </summary>
        public MainWindow()
        {
            rm = new ResourceManager(typeof(en_language));
            InitializeComponent();
            btnCreate.Content = rm.GetString("createasave");
            btnRun.Content = rm.GetString("runsave");
            btnManage.Content = rm.GetString("managesave");

            DataContext = new CreateSave(RM);
            btnCreateWasClicked = true;
            btnManageWasClicked = false;
            btnRunWasClicked = false;
            btnSettingsWasClicked = false;
            //lblIntro.Content = rm.GetString("intro");
        }
        /// <summary>
        /// Translate in English the content.
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void LanguageEN(object sender, RoutedEventArgs e)
        {
            rm = new ResourceManager(typeof(en_language));
            this.RM = this.rm;
            Translate();
            if (btnCreateWasClicked == true)
            {
                DataContext = new CreateSave(RM);
            }
            else if (btnRunWasClicked == true)
            {
                DataContext = new RunSave(RM);
            }
            else if (btnManageWasClicked == true)
            {
                DataContext = new DeleteSave(RM);
            }
            else if (btnSettingsWasClicked == true)
            {
                DataContext = new Settings(RM);
            }
            else
            {
                Translate();
            }
        }
        /// <summary>
        /// Translate in French the content.
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void LanguageFR(object sender, RoutedEventArgs e)
        {
            rm = new ResourceManager(typeof(fr_language));
            this.RM = this.rm;
            Translate();
            if (btnCreateWasClicked == true)
            {
                DataContext = new CreateSave(RM);
            }
            else if (btnRunWasClicked == true)
            {
                DataContext = new RunSave(RM);
            }
            else if (btnManageWasClicked == true)
            {
                DataContext = new DeleteSave(RM);
            }
            else if (btnSettingsWasClicked == true)
            {
                DataContext = new Settings(RM);
            }
            else
            {
                Translate();
            }
        }
        /// <summary>
        /// Show the CreateSave view.
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void CreateSaveButton(object sender, RoutedEventArgs e)
        {
            DataContext = new CreateSave(RM);
            btnCreateWasClicked = true;
            btnManageWasClicked = false;
            btnRunWasClicked = false;
            btnSettingsWasClicked = false;
        }
        /// <summary>
        /// Show the RunSave view.
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void RunSaveButton(object sender, RoutedEventArgs e)
        {
            DataContext = new RunSave(RM);
            btnRunWasClicked = true;
            btnCreateWasClicked = false;
            btnManageWasClicked = false;
            btnSettingsWasClicked = false;
        }
        /// <summary>
        /// Show the ManageSave view.
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void ManageSaveButton(object sender, RoutedEventArgs e)
        {
            DataContext = new DeleteSave(RM);
            btnManageWasClicked = true;
            btnCreateWasClicked = false;
            btnRunWasClicked = false;
            btnSettingsWasClicked = false;
        }
        /// <summary>
        /// Show the Settings view.
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void SettingsButton(object sender, RoutedEventArgs e)
        {
            lblIntro.Content = "";
            lblVersion.Content = "";
            DataContext = new Settings(RM);
            btnSettingsWasClicked = true;
            btnRunWasClicked = false;
            btnCreateWasClicked = false;
            btnManageWasClicked = false;
        }
        /// <summary>
        /// The Translate function.
        /// Translates each content of the curent view.
        /// </summary>
        public void Translate()
        {
            btnCreate.Content = rm.GetString("createasave");
            btnRun.Content = rm.GetString("runsave");
            btnManage.Content = rm.GetString("managesave");
            if (!btnSettingsWasClicked)
            lblIntro.Content = rm.GetString("intro");
        }
    }
}
