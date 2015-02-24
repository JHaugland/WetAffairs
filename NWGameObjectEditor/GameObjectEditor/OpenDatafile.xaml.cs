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
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;


namespace GameObjectEditor
{
    /// <summary>
    /// Interaction logic for OpenDatafile.xaml
    /// </summary>
    public partial class OpenDatafile : Window
    {
        private Window1 _Parent;
        public OpenDatafile(Window1 parent)
        {
            InitializeComponent();
            _Parent = parent;
            

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var list = SerializationHelper.LoadUnitClassFromXML();
            _Parent._objectList = new List<object>();
            foreach (object o in list)
            {
                _Parent._objectList.Add(o);
                _Parent.theType = o.GetType();
            }
            _Parent.lbxObjects.ItemsSource = _Parent._objectList;
            //_Parent.lbxObjects.DisplayMemberPath = "UnitClassShortName";
            
            this.Close();
        }

        private void btnWeaponClass_Click(object sender, RoutedEventArgs e)
        {
            var list = SerializationHelper.LoadWeaponClassFromXML();
            _Parent._objectList = new List<object>();
            foreach (object o in list)
            {
                _Parent._objectList.Add(o);
                _Parent.theType = o.GetType();
            }
            _Parent.lbxObjects.ItemsSource = _Parent._objectList;
            //_Parent.lbxObjects.DisplayMemberPath = "WeaponClassName";
            this.Close();

            
        }

        private void btnSensorClass_Click(object sender, RoutedEventArgs e)
        {
            var list = SerializationHelper.LoadSensorClassFromXML();
            _Parent._objectList = new List<object>();
            foreach (object o in list)
            {
                _Parent._objectList.Add(o);
                _Parent.theType = o.GetType();
            }
            _Parent.lbxObjects.ItemsSource = _Parent._objectList;
           // _Parent.lbxObjects.DisplayMemberPath = "SensorClassName";
            this.Close();
            
        }

        private void btnCountries_Click(object sender, RoutedEventArgs e)
        {
            var list = SerializationHelper.LoadCountriesFromXML();
            _Parent._objectList = new List<object>();
            foreach (object o in list)
            {
                _Parent._objectList.Add(o);
                _Parent.theType = o.GetType();
            }
            _Parent.lbxObjects.ItemsSource = _Parent._objectList;
           // _Parent.lbxObjects.DisplayMemberPath = "CountryNameShort";
            this.Close();
            
        }

        private void btnScenarios_Click(object sender, RoutedEventArgs e)
        {
            var list = SerializationHelper.LoadScenariosFromXML();
            _Parent._objectList = new List<object>();
            foreach (object o in list)
            {
                _Parent._objectList.Add(o);
                _Parent.theType = o.GetType();
            }
            _Parent.lbxObjects.ItemsSource = _Parent._objectList;
            //_Parent.lbxObjects.DisplayMemberPath = "GameName";
            this.Close();
        }

        private void btnFormations_Click(object sender, RoutedEventArgs e)
        {
            var list = SerializationHelper.LoadFormationsFromXML();
            _Parent._objectList = new List<object>();
            foreach (object o in list)
            {
                _Parent._objectList.Add(o);
                _Parent.theType = o.GetType();
            }
            _Parent.lbxObjects.ItemsSource = _Parent._objectList;
            //_Parent.lbxObjects.DisplayMemberPath = "Description";
            this.Close();
        }
    }
}
