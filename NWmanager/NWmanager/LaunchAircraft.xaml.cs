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
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for LaunchAircraft.xaml
    /// </summary>
    public partial class LaunchAircraft : Window
    {
        public List<CarriedUnitInfo> AircraftList = new List<CarriedUnitInfo>();        

        public LaunchAircraft()
        {

            InitializeComponent();
        }

        public void Init()
        {
            if (AircraftList == null || AircraftList.Count < 1)
            {
                return;
            }
            lstAircraft.Items.Clear();
            foreach (var craft in AircraftList)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = craft;
                if (craft.ReadyInSec == 0)
                {
                    item.IsEnabled = true;
                }
                else
                {
                    item.IsEnabled = false;
                }
                lstAircraft.Items.Add(item);
            }
        }

        public List<string> GetSelectedIds()
        {
            List<string> list = new List<string>();
            foreach (var o in lstAircraft.SelectedItems)
            { 
                var item = o as ListBoxItem;
                if (item != null)
                {
                    var aircraft = item.Content as CarriedUnitInfo;
                    if (aircraft != null)
                    {
                        list.Add(aircraft.Id);
                    }
                }
            }
            return list;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            lstAircraft.SelectedIndex = -1;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
