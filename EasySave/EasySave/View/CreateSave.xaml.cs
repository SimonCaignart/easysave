using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace EasySave.View
{
    /// <summary>
    /// Logique d'interaction pour CreateSave.xaml
    /// </summary>
    public partial class CreateSave : UserControl
    {
        public String saveName;
        public String sourcePath;
        public String destinationPath;
        public String saveType;

        /// <summary>
        /// Constructor
        /// Initialize the CreateSave view.
        /// </summary>
        /// <param name="RM"/>
        public CreateSave(ResourceManager RM)
        {
            InitializeComponent();
            lblSaveName.Content = RM.GetString("saveName");
            lblSourcePath.Content = RM.GetString("sourcePath");
            lblDestinationPath.Content = RM.GetString("destinationPath");
            complete.Content = RM.GetString("completeType");
            differential.Content = RM.GetString("differentialType");
        }
        /// <summary>
        /// This function removes spaces from the input
        /// </summary>
        /// <param name="e"/>
        /// <param name="sender"/>
        private void withoutSpace(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.E)
            {
                e.Handled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;
                CommonFileDialogResult rs = dlg.ShowDialog();
                if (rs == CommonFileDialogResult.Ok)
                    textSource.Text = dlg.FileName;

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;
                CommonFileDialogResult rs = dlg.ShowDialog();
                if (rs == CommonFileDialogResult.Ok)
                    textDestination.Text = dlg.FileName;

            }
        }
    }
}
