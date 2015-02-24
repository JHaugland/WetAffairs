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
using Microsoft.Win32;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWData.Util;
using System.Reflection;
using System.Reflection.Emit;
using TTG.NavalWar.NWData.OrderSystem;
using System.Collections;


namespace GameObjectEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public List<object> _objectList;
        public Type theType;
        private bool objectIsDerty;
        public Window1()
        {
            InitializeComponent();
        }

        public void Reflect()
        {
            spnlControls.Children.Clear();


            if (lbxObjects.SelectedItem != null)
            {
                Type temp = lbxObjects.SelectedItem.GetType();
                foreach (PropertyInfo pi in lbxObjects.SelectedItem.GetType().GetProperties())
                {
                    if (pi.CanWrite)
                    {
                        PropertyEditor ed = new PropertyEditor();

                        // get type and cast selected object to this type?

                        ed.lblName.Content = pi.Name;
                        Type type = lbxObjects.SelectedItem.GetType().GetProperty(pi.Name).PropertyType;
                        ed.lblName.ToolTip = pi.PropertyType.IsPublic.ToString();
                        UIElement ue = null;
                        UIElement ue2 = null;
                        switch (pi.PropertyType.Name.ToLower())
                        {
                            case "string":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    object obj = pi.GetValue(lbxObjects.SelectedItem, null);
                                    if (obj != null)
                                    {
                                        tb.Text = obj.ToString();
                                        tb.ObjectToModyfy = obj;

                                    }
                                    else
                                    {
                                        tb.ObjectToModyfy = "";
                                    }
                                    tb.PropertyName = pi.Name;
                                    tb.ToolTip = pi.PropertyType.Name;
                                    tb.Height = 20;
                                    ue = tb;
                                    break;
                                }
                            case "list`1":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    Type tTemp = GetGenericType(pi.GetValue(lbxObjects.SelectedItem, null));

                                    tb.Text = tTemp.Name;
                                    tb.ToolTip = "(" + tTemp.Name + ") list<>";

                                    tb.PropertyName = pi.Name;
                                    tb.Height = 20;
                                    tb.IsEnabled = false;
                                    ButtonExt btn = new ButtonExt();
                                    btn.ObjectToModyfy = pi.GetValue(lbxObjects.SelectedItem, null);
                                    btn.PropertyName = pi.Name;
                                    btn.Content = "[..]";
                                    btn.HorizontalAlignment = HorizontalAlignment.Left;
                                    btn.Click += new RoutedEventHandler(btn_Click);
                                    btn.Width = 20;
                                    ue2 = btn;
                                    ue = tb;

                                    break;
                                }
                            case "double":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    tb.Text = pi.GetValue(lbxObjects.SelectedItem, null).ToString();
                                    tb.PropertyName = pi.Name;
                                    tb.ObjectToModyfy = pi.GetValue(lbxObjects.SelectedItem, null);
                                    if (tb.ObjectToModyfy == null)
                                    {
                                        tb.ObjectToModyfy = "";
                                    }
                                    tb.Height = 20;
                                    tb.ToolTip = pi.PropertyType.Name;
                                    ue = tb;
                                    break;
                                }
                            case "int32":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    tb.Text = pi.GetValue(lbxObjects.SelectedItem, null).ToString();
                                    tb.PropertyName = pi.Name;
                                    tb.ObjectToModyfy = pi.GetValue(lbxObjects.SelectedItem, null);
                                    tb.Height = 20;
                                    tb.ToolTip = pi.PropertyType.Name;
                                    ue = tb;
                                    break;
                                }
                            case "boolean":
                                {
                                    CheckBoxExt chk = new CheckBoxExt();
                                    chk.PropertyName = pi.Name;
                                    object obj = pi.GetValue(lbxObjects.SelectedItem, null);
                                    chk.ObjectToModyfy = obj;
                                    chk.IsChecked = (bool)obj;
                                    chk.ToolTip = pi.PropertyType.Name;
                                    ue = chk;
                                    break;
                                }
                            default:
                                {

                                    if (pi.PropertyType.IsEnum)
                                    {
                                        ComboBoxExt bx = new ComboBoxExt();
                                        bx.PropertyName = pi.Name;
                                        bx.DisplayMemberPath = "Name";
                                        bx.ToolTip = pi.PropertyType.Name;
                                        foreach (FieldInfo feif in pi.PropertyType.GetFields())
                                        {
                                            if (feif.Name != "value__")
                                            {
                                                bx.Items.Add(feif);
                                            }
                                        }
                                        bx.ObjectToModyfy = pi.GetValue(lbxObjects.SelectedItem, null);
                                        bx.SelectedItem = pi.PropertyType.GetField(pi.GetValue(lbxObjects.SelectedItem, null).ToString());
                                        ue = bx;
                                    }
                                    else if (pi.PropertyType.IsGenericType &&
                                              pi.PropertyType.GetGenericTypeDefinition() ==
                                              typeof(Nullable<>))
                                    {
                                        TextBox tb = new TextBox();
                                        Type tTemp = GetGenericType(pi.GetValue(lbxObjects.SelectedItem, null));

                                        Type propType = pi.PropertyType;
                                        if (propType.IsGenericType &&
                                              propType.GetGenericTypeDefinition() ==
                                              typeof(Nullable<>))
                                        {
                                            Type[] typeCol = propType.GetGenericArguments();
                                            Type nullableType;
                                            if (typeCol.Length > 0)
                                            {
                                                nullableType = typeCol[0];
                                                if (nullableType.BaseType == typeof(Enum))
                                                {
                                                    ComboBox bx = new ComboBox();
                                                    foreach (FieldInfo feif in nullableType.GetFields())
                                                    {
                                                        if (feif.Name != "value__")
                                                        {
                                                            bx.Items.Add(feif);
                                                        }
                                                    }
                                                    ue = bx;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ButtonExt btn = new ButtonExt();
                                        btn.PropertyName = pi.Name;
                                        btn.Content = pi.PropertyType.Name;
                                        btn.ObjectToModyfy = pi.GetValue(lbxObjects.SelectedItem, null);
                                        btn.EditingType = pi.PropertyType;
                                        btn.Click += new RoutedEventHandler(btn_ClickOeditor);
                                        ue = btn;
                                    }
                                    break;
                                }
                        }
                        if (ue != null)
                        {
                            ue.SetValue(Grid.ColumnProperty, 1);
                            ue.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
                            ed.ContentGrid.Children.Add(ue);
                            if (ue2 != null)
                            {
                                ue2.SetValue(Grid.ColumnProperty, 2);
                                ue2.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
                                ed.ContentGrid.Children.Add(ue2);
                            }
                        }

                        spnlControls.Children.Add(ed);
                    }
                }
            }

        }
        void btn_ClickOeditor(object sender, RoutedEventArgs e)
        {
            ButtonExt btn = sender as ButtonExt;
            if (btn.ObjectToModyfy == null)
            {
                btn.ObjectToModyfy = Activator.CreateInstance(btn.EditingType);
            }
            ObjectEditor oe = new ObjectEditor(btn.EditingType, btn.ObjectToModyfy);
            oe.ShowDialog();
        }
        private void UpdateObject(object e)
        {
            int index = lbxObjects.Items.IndexOf(e);
            if (index == -1)
                return;
            foreach (PropertyEditor ed in spnlControls.Children)
            {
                foreach (Control c in ed.ContentGrid.Children)
                {
                    if (c.GetType() == typeof(System.Windows.Controls.Label))
                    {
                        continue;
                    }
                    if (c.GetType() == typeof(ButtonExt))
                    {
                        ButtonExt bx = c as ButtonExt;
                        
                        PropertyInfo pi = lbxObjects.Items[index].GetType().GetProperty(bx.PropertyName);
                        if (bx.ObjectToModyfy != null)
                        {
                            pi.SetValue(lbxObjects.Items[index], bx.ObjectToModyfy, null);
                        }
                    }
                    else if (c.GetType() == typeof(TextboxExt))
                    {
                        TextboxExt bx = c as TextboxExt;
                        PropertyInfo pi = lbxObjects.Items[index].GetType().GetProperty(bx.PropertyName);
                        if (bx.ObjectToModyfy != null)
                        {
                            if (pi.PropertyType.Name.ToLower() == "string")
                            {
                                pi.SetValue(lbxObjects.Items[index], bx.Text, null);
                            }
                            else if (pi.PropertyType.Name.ToLower() == "int32")
                            {
                                string temp = bx.Text.Replace(",", ".");//;
                                pi.SetValue(lbxObjects.Items[index], Convert.ToInt32(temp), null);
                            }
                            else if (pi.PropertyType.Name.ToLower() == "double")
                            {
                                string temp = bx.Text.Replace(".", ",");
                                pi.SetValue(lbxObjects.Items[index], Convert.ToDouble(temp), null);
                            }
                            else if (pi.PropertyType.Name.ToLower() == "float")
                            {
                                string temp = bx.Text.Replace(".", ",");
                                pi.SetValue(lbxObjects.Items[index], Convert.ToDouble(temp), null);
                            }
                        }

                    }
                    else if (c.GetType() == typeof(CheckBoxExt))
                    {
                        CheckBoxExt bx = c as CheckBoxExt;
                        PropertyInfo pi = lbxObjects.Items[index].GetType().GetProperty(bx.PropertyName);
                        pi.SetValue(lbxObjects.Items[index], bx.IsChecked, null);
                    }
                    else if (c.GetType() == typeof(ComboBoxExt))
                    {
                        ComboBoxExt bx = c as ComboBoxExt;
                        PropertyInfo pin = lbxObjects.Items[index].GetType().GetProperty(bx.PropertyName);
                        if (bx.ObjectToModyfy != null)
                        {
                            FieldInfo fofi = bx.SelectedItem as FieldInfo;
                            string name = fofi.Name;
                            pin.SetValue(lbxObjects.Items[index], Enum.Parse(pin.PropertyType, name), null);
                        }
                    }

                }
            }
            lbxObjects.Items.Refresh();
        }

        private void SaveToXML()
        {
            
            switch (theType.Name)
            {
                    
                case "UnitClass":
                    {
                        Serializer<UnitClass> unitClassXml = new Serializer<UnitClass>();
                        List<UnitClass> unitClassList = new List<UnitClass>();
                        foreach (object o in _objectList)
                        {
                            unitClassList.Add((UnitClass)o);
                        }
                        unitClassXml.SaveListToXML(unitClassList, SerializationHelper.UNIT_CLASSES_FILENAME);
                        break;
                    }
                case "WeaponClass":
                    {
                        Serializer<WeaponClass> WeaponClassXml = new Serializer<WeaponClass>();
                        List<WeaponClass> WeaponClassList = new List<WeaponClass>();
                        foreach (object o in _objectList)
                        {
                            WeaponClassList.Add((WeaponClass)o);
                        }
                        WeaponClassXml.SaveListToXML(WeaponClassList, SerializationHelper.WEAPON_CLASSES_FILENAME);
                        break;
                    }
                case "SensorClass":
                    {
                        Serializer<SensorClass> SensorClassXml = new Serializer<SensorClass>();
                        List<SensorClass> SensorClassList = new List<SensorClass>();
                        foreach (object o in _objectList)
                        {
                            SensorClassList.Add((SensorClass)o);
                        }
                        SensorClassXml.SaveListToXML(SensorClassList, SerializationHelper.SENSOR_CLASSES_FILENAME);
                        break;
                    }
                case "Country":
                    {
                        Serializer<Country> CountryXml = new Serializer<Country>();
                        List<Country> CountryList = new List<Country>();
                        foreach (object o in _objectList)
                        {
                            CountryList.Add((Country)o);
                        }
                        CountryXml.SaveListToXML(CountryList, SerializationHelper.COUNTRIES_FILENAME);
                        break;
                    }
                case "GameScenario":
                    {
                        Serializer<GameScenario> GameScenarioXml = new Serializer<GameScenario>();
                        List<GameScenario> GameScenarioList = new List<GameScenario>();
                        foreach (object o in _objectList)
                        {
                            GameScenarioList.Add((GameScenario)o);
                        }
                        GameScenarioXml.SaveListToXML(GameScenarioList, SerializationHelper.SENSOR_CLASSES_FILENAME);
                        break;
                    }
                case "Formation":
                    {
                        Serializer<Formation> FormationXml = new Serializer<Formation>();
                        List<Formation> FormationList = new List<Formation>();
                        foreach (object o in _objectList)
                        {
                            FormationList.Add((Formation)o);
                        }
                        FormationXml.SaveListToXML(FormationList, SerializationHelper.FORMATIONS_FILENAME);
                        break;
                    }
                default:
                    break;
            }

        }


        void btn_Click(object sender, RoutedEventArgs e)
        {

            ButtonExt b = sender as ButtonExt;

            object t = b.ObjectToModyfy as object;

            CollectionEditor ed = new CollectionEditor(t, b.PropertyName);

            ed.ShowDialog();

            if (ed.ShouldSave == true)
            {
                PropertyInfo pi = lbxObjects.SelectedItem.GetType().GetProperty(ed.editingPropertyName);
               
                //int itemCount = (int)obj.GetType().GetProperty("Count").GetValue(obj, null);
                Type type = lbxObjects.SelectedItem.GetType();
                PropertyInfo pif = lbxObjects.SelectedItem.GetType().GetProperty(ed.editingPropertyName);
                MethodInfo clearMethod = pif.GetValue(lbxObjects.SelectedItem,null).GetType().GetMethod("Clear");
                MethodInfo addMethod = pif.GetValue(lbxObjects.SelectedItem, null).GetType().GetMethod("Add");
                clearMethod.Invoke(lbxObjects.SelectedItem.GetType().GetProperty(ed.editingPropertyName).GetValue(lbxObjects.SelectedItem, null), null);
                //object clearvalue = propertyType.Invoke(lbxObjects.SelectedItem, null);

                foreach (var v in ed.ObjectList)
                {
                    object[] param = new object[1];
                    param[0] = v;
                    addMethod.Invoke(lbxObjects.SelectedItem.GetType().GetProperty(ed.editingPropertyName).GetValue(lbxObjects.SelectedItem, null), param);
                }
                
            }


        }
        



        private Type GetGenericType(object obj)
        {
            if (obj != null)
            {
                Type t = obj.GetType();
                if (t.IsGenericType)
                {
                    Type[] at = t.GetGenericArguments();
                    t = at.First<Type>();
                } return t;
            }
            else
            {
                return null;
            }
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mn = (MenuItem)e.OriginalSource;
            string itemName = mn.Header.ToString();

            switch (itemName)
            {
                case "Open":
                    {
                        OpenDatafile odf = new OpenDatafile(this);
                        odf.Show();
                        break;
                    }
                case "Save":
                    {
                        SaveToXML();
                        break;
                    }
                case "New":
                    {
                        object o = Activator.CreateInstance(theType);
                        _objectList.Add(o);
                        lbxObjects.Items.Refresh();
                        

                        break;
                    }
                default:
                    break;
            }



        }

        private void lbxObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            object removed = null;
            if (e.RemovedItems.Count >= 1)
                removed = e.RemovedItems[0];
            if (removed != null)
            {
                UpdateObject(removed);
            }
            Reflect();
        }

        private void tbToolBar_GotFocus(object sender, RoutedEventArgs e)
        {
            


            UpdateObject(lbxObjects.SelectedItem);
        }



    }
}
