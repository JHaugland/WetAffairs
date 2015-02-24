using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for ClientUnitInfo.xaml
    /// </summary>
    public partial class ClientUnitInfo : Window
    {
        public ClientUnitInfo()
        {
            InitializeComponent();
        }

        public void ShowInfo(string info)
        {
            txtUnitInfo.Text = info;
        }

        public void ShowDetectedUnitInfo(string info)
        {
            txtDetectedUnitInfo.Text = info;
        }
    }
}
