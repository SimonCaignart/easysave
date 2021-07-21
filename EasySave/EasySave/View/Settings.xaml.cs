using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Resources;

namespace EasySave.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings(ResourceManager RM)
        {
            InitializeComponent();

            //CryptedExtension
            labelCryptedExtensions.Content = RM.GetString("CryptoSoftSettings");
            ExtensionList.Content = RM.GetString("ExtensionList");
            labelAddExtension.Content = RM.GetString("labelAddExtension");
            ButtonAddExtension.Content = RM.GetString("Add");
            ButtonDeleteExtension.Content = RM.GetString("btnDelete");

            //Crypto Key
            labelCryptoKey.Content = RM.GetString("labelCryptoKey");
            btnModifyCryptoKey.Content = RM.GetString("Modify");

            //Max File Size
            labelMaxFileSize.Content = RM.GetString("labelMaxSize");
            labelMaxSize.Content = RM.GetString("labelMaxFileSize");
            btnModifyMaxFileSize.Content = RM.GetString("Modify");
            labelInfoMaxFileSize.Content = RM.GetString("labelInfoMaxFileSize");

            //WorkSoftware
            labelWorkSoftware.Content = RM.GetString("labelWorkSoftware");
            labelListWorkSoftware.Content = RM.GetString("labelListWorkSoftware");
            labelListWorkSoftware2.Content = RM.GetString("labelListWorkSoftware2");

            //Priority Extensions
            labelPriorityExtensions.Content = RM.GetString("PriorityExtensions");
            PriorityExtensionList.Content = RM.GetString("PriorityExtensionList");
            labelAddPriorityExtension.Content = RM.GetString("labelAddPriorityExtension");
            ButtonAddPriorityExtension.Content = RM.GetString("Add");
            ButtonDeletePriorityExtension.Content = RM.GetString("btnDelete");

        }
    }
}
