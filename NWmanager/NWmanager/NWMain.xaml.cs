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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using System.IO;

namespace NWmanager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NWMain : Window
    {
        
        public NWMain()
        {
            InitializeComponent();
        }

        private void ShowMessage(string message)
        {
            if (message == string.Empty)
            {
                txtInfo.Text = string.Empty;
                return;
            }
            txtInfo.Text += message + "\n";
        }

        private void btnCreateTestXML_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This will delete all existing data files. Are you sure?", "Please confirm", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
            {
                ShowMessage("Create canceled by user.");
                return;
            }
            UnitDatabaseFactory.CreateUnitDatabase( ShowMessage );
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            MapVisulizer MPVZ = new MapVisulizer();
            MPVZ.Show();
        }

        private void btnCommsTest_Click(object sender, RoutedEventArgs e)
        {
            CommsClient client = new CommsClient();
            client.Show();
        }

        private void cmbLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item;
            string tag = string.Empty;
            if(cmbLists.SelectedItem != null)
            {
                item = cmbLists.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    tag = (string)item.Tag;
                }
            }
            if (string.IsNullOrEmpty(tag))
            {
                ShowMessage("\nNothing selected");
                return;
            }
            Player player = new Player();
            player.Name = "Testski";
            player.IsComputerPlayer = true;
            GameManager.Instance.GameData.InitAllData();
            txtInfo.Text = string.Empty;
            switch (tag)
            {
                case "U":
                    ShowMessage("\nUnitClasses:\n"); 
                    foreach (var u in GameManager.Instance.GameData.UnitClasses)
                    {
                        ShowMessage(u.Id.PadRight(30, ' ') + "\t*" 
                            + u.UnitModelFileName.PadRight(30, ' ') 
                            + "\t" + u.UnitClassShortName.PadRight(60, ' ') 
                            + " \t " + u.UnitType);
                    }
                    break;
                case"E": //Extended Unit Classes
                    ShowMessage("\nUnitClasses: (Extended)\n"); 
                    foreach (var u in GameManager.Instance.GameData.UnitClasses)
                    {
                        ShowMessage(u.Id.PadRight(30, ' ') + "\t" 
                            + u.UnitClassLongName.PadRight(60, ' ') 
                            + "\t" + u.UnitClassShortName.PadRight(60, ' ') 
                            + " \t " + u.UnitType);
                        ShowMessage("\nSensors:");
                        foreach (var sens in u.SensorClassIdList)
                        {
                            var sensor = GameManager.Instance.GameData.SensorClasses.FirstOrDefault(s => s.Id == sens);
                            ShowMessage("\t" + sensor);
                        }
                        ShowMessage("\nWeaponLoads:");
                        foreach (var wls in u.WeaponLoads)
                        {
                            ShowMessage("\t" + wls.Name + ": ");
                            foreach (var wl in wls.WeaponLoads)
                            {
                                var wpn = GameManager.Instance.GameData.WeaponClasses.FirstOrDefault(s=>s.Id == wl.WeaponClassId);
                                ShowMessage("\t\t" + wl.WeaponClassId + ": " + wpn + "(" + wl.MaxAmmunition + ")\t " + (wpn.MaxWeaponRangeM /1000).ToString("#0.0"));
                            }
                        }
                        ShowMessage(" ");
                    }

                    break;
                case "W":
                    ShowMessage("\nWeaponClasses:\n"); 
                    foreach (var u in GameManager.Instance.GameData.WeaponClasses)
                    {
                        var modelFileName = "";
                        if (!string.IsNullOrEmpty(u.UnitModelFileName))
                        {
                            modelFileName = u.UnitModelFileName;
                        }
                        var targetType = "";
                        if (u.CanTargetAir)
                        {
                            targetType += " air ";
                        }
                        if (u.CanTargetSurface)
                        {
                            targetType += " sur ";
                        }
                        if (u.CanTargetSubmarine)
                        {
                            targetType += " sub ";
                        }
                        if (u.CanTargetLand)
                        {
                            targetType += " lan ";
                        }

                        ShowMessage(u.Id.PadRight(40, ' ') + "\t" 
                            + u.SpawnUnitClassId.PadRight(40, ' ') + "\t" 
                            + modelFileName.PadRight(40,' ') + " \t " + u.WeaponClassName + "\t" 
                            + Math.Round(u.MaxWeaponRangeM / 1000,0).ToString().PadRight(10,' ') + "\t"
                            + (u.MaxSpeedKph / 1000).ToString("#0.0").PadRight(12,' ') + "\t"
                            + targetType.PadRight(20));
                    }
                    break;

                case "S":
                    ShowMessage("\nSensorClasses:\n"); 
                    foreach (var u in GameManager.Instance.GameData.SensorClasses)
                    {
                        ShowMessage(u.Id.PadRight(30, ' ') + "\t " 
                            + u.SensorClassName.PadRight(30, ' ') + "\t (" + u.SensorType + ")");
                    }
                    break;

                case "F":
                    ShowMessage("\nFormations:\n"); 
                    foreach (var u in GameManager.Instance.GameData.Formations)
                    {
                        ShowMessage(u.Id.PadRight(30, ' ') + " \t " + u.Description);
                        foreach (var p in u.FormationPositions)
                        {
                            ShowMessage("\t" + p.Id.PadRight(30, ' ') + "\t " + p.PositionOffset.ToString());
                        }
                    }
                    break;

                case "C": //sCenaarios
                    ShowMessage("\nScenarios:\n");
                    foreach (var u in GameManager.Instance.GameData.Scenarios)
                    {
                        ShowMessage(u.Id.PadRight(30, ' ') + " \t " 
                            + u.GameName.PadRight(30, ' ') + " \t ");
                    }
                    break;

                default:
                    ShowMessage("\nNothing selected.\n");
                    break;
            }
        }

    }
}
