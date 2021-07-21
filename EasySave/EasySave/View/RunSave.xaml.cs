using System;
using System.Collections.Generic;
using System.Resources;
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

namespace EasySave.View
{
    /// <summary>
    /// Logique d'interaction pour RunSave.xaml
    /// </summary>
    public partial class RunSave : UserControl
    {
        public RunSave(ResourceManager RM)
        {
            InitializeComponent();
            btnRunAll.Content = RM.GetString("runallsaves");
            lblList.Content = RM.GetString("listallsaves");
            btnRunSpecific.Content = RM.GetString("runspecificsave");
        }
    }
}
