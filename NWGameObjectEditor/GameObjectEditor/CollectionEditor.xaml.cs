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
using System.Reflection;
using TTG.NavalWar.NWComms.Entities;
using System.Collections;
using TTG.NavalWar.NWComms;

namespace GameObjectEditor
{
    /// <summary>
    /// Interaction logic for CollectionEditor.xaml
    /// </summary>
    public partial class CollectionEditor : Window
    {
        public object editingObject;
        public Type editingType;
        public string editingPropertyName;
        public List<object> ObjectList;
        public bool ShouldSave = true;
        private bool ObjectIsDirty = false;

        public CollectionEditor()
        {
            InitializeComponent();
        }

        public CollectionEditor(object obj, string propertyName)
        {
            InitializeComponent();
            this.Title += " - " + propertyName;
            editingPropertyName = propertyName;
            Type type = GetGenericType(obj);
            if (type.IsEnum)
            {
                PropertyInfo[] pi = obj.GetType().GetProperties();


                foreach (FieldInfo feif in pi[2].PropertyType.GetFields())
                {
                    if (feif.Name != "value__")
                    {
                        MenuItem item = new MenuItem();
                        item.Header = feif.Name;
                        mnuNewItem.Items.Add(item);
                    }
                }

            }
            editingType = type;
            ObjectList = new List<object>();

            int itemCount = (int)obj.GetType().GetProperty("Count").GetValue(obj, null);

            MemberInfo[] mif = obj.GetType().GetDefaultMembers();
            object[] memb = new object[1];
            for (int i = 0; i < itemCount; i++)
            {
                memb[0] = i;
                object temp0 = obj.GetType().GetProperty("Item").GetValue(obj, memb);

                ObjectList.Add(temp0);
            }

            lbxEnteties.ItemsSource = ObjectList;

        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mn = (MenuItem)e.OriginalSource;
            string itemName = mn.Header.ToString();

            switch (itemName)
            {
                case "New":
                    {
                        object o;
                        if (editingType == typeof(String))
                        {
                            o = "New String";
                        }
                        else
                        {
                           o = Activator.CreateInstance(editingType);
                        }
                        ObjectList.Add(o);
                        lbxEnteties.Items.Refresh();
                        ObjectIsDirty = true;
                        break;
                    }
                default:
                    {
                        if (editingType.IsEnum)
                        {
                            object o = Enum.Parse(editingType, itemName, true);
                            ObjectList.Add(o);
                            lbxEnteties.Items.Refresh();
                            ObjectIsDirty = true;
                        }
                        break;
                    }
                    
            }
        }
        private Type GetGenericType(object obj)
        {
            if (obj == null)
                return null;
            Type t = obj.GetType();
            if (t.IsGenericType)
            {
                t = t.GetGenericArguments()[0];
            }
            return t;
        }
        
        public List<T> MakeList<T>(T type, List<object> list)
        {                        
            List<T> ef = new List<T>();
            foreach (T o in list)
            {
                ef.Add((T)o);
            }
            return ef.ToList<T>();
        } 

        public void Reflect()
        {
            spnlControls.Children.Clear();


            if (lbxEnteties.SelectedItem != null)
            {
                Type temp = lbxEnteties.SelectedItem.GetType();
                
                if (temp == typeof(String))
                {
                    PropertyEditor ed = new PropertyEditor();
                    TextboxExt tb = new TextboxExt();
                    tb.Width = 150;
                    tb.ObjectToModyfy = lbxEnteties.SelectedItem.ToString();
                    tb.Text = lbxEnteties.SelectedItem.ToString();
                    UIElement ue = tb;
                    ue.SetValue(Grid.ColumnProperty, 1);
                    ue.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
                    ed.ContentGrid.Children.Add(ue);
                    spnlControls.Children.Add(ed);
                    return;
                }
                foreach (PropertyInfo fi in lbxEnteties.SelectedItem.GetType().GetProperties())
                {
                    if (fi.CanWrite)
                    {
                       
                        Type type = fi.PropertyType.GetType(); ;

                        // get type and cast selected object to this type?
                        PropertyEditor ed = new PropertyEditor();
                        ed.lblName.Content = fi.Name;
                        ed.lblName.ToolTip = fi.PropertyType.IsPublic.ToString();
                        UIElement ue = null;
                        UIElement ue2 = null;
                        switch (fi.PropertyType.Name.ToLower())
                        {
                            case "string":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    object obj = fi.GetValue(lbxEnteties.SelectedItem, null);
                                    if (obj != null)
                                    {
                                        tb.Text = obj.ToString();
                                        tb.ObjectToModyfy = obj;
                                    }
                                    else
                                    {
                                        tb.ObjectToModyfy = "";
                                    }
                                    tb.PropertyName = fi.Name;
                                    tb.ToolTip = fi.PropertyType.Name;
                                    tb.Height = 20;
                                    ue = tb;
                                    break;
                                }
                            case "list`1":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    Type tTemp = GetGenericType(fi.GetValue(lbxEnteties.SelectedItem, null));
                                    tb.PropertyName = fi.Name;
                                    tb.Text = tTemp.Name;
                                    tb.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                    tb.Height = 20;
                                    tb.ToolTip = "(" + tTemp.Name + ") list<>";
                                    // tb.IsEnabled = false;
                                    ButtonExt btn = new ButtonExt();
                                    btn.Tag = fi.GetValue(lbxEnteties.SelectedItem, null);
                                    btn.PropertyName = fi.Name;
                                    btn.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
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
                                    tb.Text = fi.GetValue(lbxEnteties.SelectedItem, null).ToString();
                                    tb.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                    if (tb.ObjectToModyfy == null)
                                    {
                                        tb.ObjectToModyfy = "";
                                    }
                                    tb.Height = 20;
                                    tb.PropertyName = fi.Name;
                                    tb.ToolTip = fi.PropertyType.Name;
                                    ue = tb;
                                    break;
                                }
                            case "int32":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    tb.Text = fi.GetValue(lbxEnteties.SelectedItem, null).ToString();
                                    tb.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                    if (tb.ObjectToModyfy == null)
                                    {
                                        tb.ObjectToModyfy = "";
                                    }
                                    tb.Height = 20;
                                    tb.ToolTip = fi.PropertyType.Name;
                                    tb.PropertyName = fi.Name;
                                    ue = tb;
                                    break;
                                }
                            case "boolean":
                                {
                                    CheckBoxExt chk = new CheckBoxExt();
                                    chk.IsChecked = (bool)fi.GetValue(lbxEnteties.SelectedItem, null);
                                    chk.ToolTip = fi.PropertyType.Name;
                                    chk.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                    if (chk.ObjectToModyfy == null)
                                    {
                                        chk.ObjectToModyfy = false;
                                    }
                                    chk.PropertyName = fi.Name;
                                    ue = chk;
                                    break;
                                }
                            default:
                                {

                                    if (fi.PropertyType.IsEnum)
                                    {
                                        ComboBoxExt bx = new ComboBoxExt();
                                        bx.DisplayMemberPath = "Name";
                                        bx.ToolTip = fi.PropertyType.Name;
                                        bx.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                        bx.PropertyName = fi.Name;
                                        foreach (FieldInfo feif in fi.PropertyType.GetFields())
                                        {
                                            if (feif.Name != "value__")
                                            {
                                                bx.Items.Add(feif);
                                            }
                                        }


                                        bx.SelectedItem = fi.GetValue(lbxEnteties.SelectedItem, null);

                                        ue = bx;
                                    }
                                    else if (fi.PropertyType.IsGenericType &&
                                              fi.PropertyType.GetGenericTypeDefinition() ==
                                              typeof(Nullable<>))
                                    {
                                        TextboxExt tb = new TextboxExt();
                                        Type tTemp = GetGenericType(fi.GetValue(lbxEnteties.SelectedItem, null));
                                        tb.PropertyName = fi.Name;
                                        tb.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                        Type propType = fi.PropertyType;
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
                                                    ComboBoxExt bx = new ComboBoxExt();
                                                    bx.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                                    bx.PropertyName = fi.Name;
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
                                        btn.PropertyName = fi.Name;
                                        btn.Content = fi.Name;
                                        btn.ObjectToModyfy = fi.GetValue(lbxEnteties.SelectedItem, null);
                                        btn.EditingType = fi.PropertyType;
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
                        if(ed != null)
                        {
                        spnlControls.Children.Add(ed);
                        }
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

        void btn_Click(object sender, RoutedEventArgs e)
        {
            ButtonExt b = sender as ButtonExt;
            object t = b.Tag as object;

            CollectionEditor ed = new CollectionEditor(t, b.PropertyName);

            ed.ShowDialog();
        }

        private void lbxEnteties_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        private void UpdateObject(object e)
        {
            int index = lbxEnteties.Items.IndexOf(e);
            Type temp5 = e.GetType();
            
            if (temp5 == typeof(String))
            {
                foreach (PropertyEditor edi in spnlControls.Children)
                {
                    foreach (Control c in edi.ContentGrid.Children)
                    {
                        if (c.GetType() == typeof(TextboxExt))
                        {
                            TextBox t = c as TextBox;
                           int ei = ObjectList.IndexOf(e);
                           ObjectList[ei] = t.Text;// = t.Text;
                        }
                    }
                }
                lbxEnteties.Items.Refresh();
                return;
            }
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

                        PropertyInfo pi = lbxEnteties.Items[index].GetType().GetProperty(bx.PropertyName);
                        if (bx.ObjectToModyfy != null)
                        {
                            pi.SetValue(lbxEnteties.Items[index], bx.ObjectToModyfy, null);
                        }
                    }
                    else if (c.GetType() == typeof(TextboxExt))
                    {
                        TextboxExt bx = c as TextboxExt;
                        PropertyInfo pi = lbxEnteties.Items[index].GetType().GetProperty(bx.PropertyName);
                        if (bx.ObjectToModyfy != null)
                        {
                            if (pi.PropertyType.Name.ToLower() == "string")
                            {
                                pi.SetValue(lbxEnteties.Items[index], bx.Text, null);
                            }
                            else if (pi.PropertyType.Name.ToLower() == "int32")
                            {
                                string temp = bx.Text.Replace(",", ".");//;
                                pi.SetValue(lbxEnteties.Items[index], Convert.ToInt32(temp), null);
                            }
                            else if (pi.PropertyType.Name.ToLower() == "double")
                            {
                                string temp = bx.Text.Replace(".", ",");
                                pi.SetValue(lbxEnteties.Items[index], Convert.ToDouble(temp), null);
                            }
                            else if (pi.PropertyType.Name.ToLower() == "float")
                            {
                                string temp = bx.Text.Replace(".", ",");
                                pi.SetValue(lbxEnteties.Items[index], Convert.ToDouble(temp), null);
                            }
                        }

                    }
                    else if (c.GetType() == typeof(CheckBoxExt))
                    {
                        CheckBoxExt bx = c as CheckBoxExt;
                        PropertyInfo pi = lbxEnteties.Items[index].GetType().GetProperty(bx.PropertyName);
                        pi.SetValue(lbxEnteties.Items[index], bx.IsChecked, null);
                    }
                    else if (c.GetType() == typeof(ComboBoxExt))
                    {
                        ComboBoxExt bx = c as ComboBoxExt;
                        PropertyInfo pin = lbxEnteties.Items[index].GetType().GetProperty(bx.PropertyName);
                        if (bx.ObjectToModyfy != null)
                        {
                            FieldInfo fofi = bx.SelectedItem as FieldInfo;// NUllable???
                            if (fofi != null)
                            {
                                string name = fofi.Name;
                                Type genType = GetGenericType(pin.GetValue(lbxEnteties.Items[index], null));
                                pin.SetValue(lbxEnteties.Items[index], Enum.Parse(genType, name), null);//pin.PropertyType
                            }
                        }
                    }

                }
            }
            lbxEnteties.Items.Refresh();
        }

        private void Canel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Quit without saving changes?", "Quit?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
               if(result == MessageBoxResult.Yes)
               {
                ShouldSave = false;
                this.Close();
               }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ShouldSave = true;
            this.Close();
        }

    }
}
