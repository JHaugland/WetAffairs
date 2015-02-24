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
using System.Reflection;

namespace GameObjectEditor
{
    /// <summary>
    /// Interaction logic for ObjectEditor.xaml
    /// </summary>
    public partial class ObjectEditor : Window
    {
        public Type objectType;
        public object theObject;
        public ObjectEditor()
        {
            InitializeComponent();

        }
        public ObjectEditor(Type theType, object editObject)
        {
            InitializeComponent();
            objectType = theType;
            theObject = editObject;

            Reflect();



        }

        public void Reflect()
        {
            PropertyInfo[] pit;
            if (theObject != null)
            {
                pit = theObject.GetType().GetProperties();
            }
            else
            {
                pit = objectType.GetProperties();
            }

                foreach (PropertyInfo pi in pit)
                {
                    if (pi.CanWrite)
                    {
                        PropertyEditor ed = new PropertyEditor();

                        // get type and cast selected object to this type?

                        ed.lblName.Content = pi.Name;
                        //Type type = theObject.GetType().GetProperty(pi.Name).PropertyType;
                        ed.lblName.ToolTip = pi.PropertyType.IsPublic.ToString();
                        UIElement ue = null;
                        UIElement ue2 = null;
                        switch (pi.PropertyType.Name.ToLower())
                        {
                            case "string":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    object obj = pi.GetValue(theObject, null);
                                    if (obj != null)
                                    {
                                        tb.Text = obj.ToString();
                                        tb.ObjectToModyfy = obj;

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
                                    Type tTemp = GetGenericType(pi.GetValue(theObject, null));

                                    tb.Text = tTemp.Name;
                                    tb.ToolTip = "(" + tTemp.Name + ") list<>";

                                    tb.PropertyName = pi.Name;
                                    tb.Height = 20;
                                    tb.IsEnabled = false;
                                    ButtonExt btn = new ButtonExt();
                                    btn.ObjectToModyfy = pi.GetValue(theObject, null);
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
                                    tb.Text = theObject == null ? null : pi.GetValue(theObject, null).ToString();
                                    tb.PropertyName = pi.Name;
                                    tb.ObjectToModyfy = theObject == null ? null : pi.GetValue(theObject, null);
                                    tb.Height = 20;
                                    tb.ToolTip = pi.PropertyType.Name;
                                    ue = tb;
                                    break;
                                }
                            case "int32":
                                {
                                    TextboxExt tb = new TextboxExt();
                                    tb.Text = pi.GetValue(theObject, null).ToString();
                                    tb.PropertyName = pi.Name;
                                    tb.ObjectToModyfy = pi.GetValue(theObject, null);
                                    tb.Height = 20;
                                    tb.ToolTip = pi.PropertyType.Name;
                                    ue = tb;
                                    break;
                                }
                            case "boolean":
                                {
                                    CheckBoxExt chk = new CheckBoxExt();
                                    chk.PropertyName = pi.Name;
                                    object obj = pi.GetValue(theObject, null);
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
                                        bx.ObjectToModyfy = pi.GetValue(theObject, null);
                                        bx.SelectedItem = pi.PropertyType.GetField(pi.GetValue(theObject, null).ToString());
                                        ue = bx;
                                    }
                                    else if (pi.PropertyType.IsGenericType &&
                                              pi.PropertyType.GetGenericTypeDefinition() ==
                                              typeof(Nullable<>))
                                    {
                                        TextBox tb = new TextBox();
                                        Type tTemp = GetGenericType(pi.GetValue(theObject, null));

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
                                        if (pi.PropertyType == typeof(object))
                                        {
                                        }
                                    }
                                    break;
                                }
                        }
                        if (ue != null)
                        {
                           // ue.SetValue(Grid.ColumnProperty, 0);
                            ue.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
                            ed.ContentGrid.Children.Add(ue);
                            if (ue2 != null)
                            {
                                //ue2.SetValue(Grid.ColumnProperty, 0);
                                ue2.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
                                ed.ContentGrid.Children.Add(ue2);
                            }
                        }

                        spnlControls.Children.Add(ed);
                    }
                }
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            ButtonExt b = sender as ButtonExt;
            object t = b.Tag as object;

            CollectionEditor ed = new CollectionEditor(t, b.PropertyName);

            ed.ShowDialog();
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (PropertyEditor edi in spnlControls.Children)
            {
                foreach (Control c in edi.ContentGrid.Children)
                {
                    if (c.GetType() == typeof(TextboxExt))
                    {
                        TextboxExt bx = c as TextboxExt;
                        PropertyInfo pi = theObject.GetType().GetProperty(bx.PropertyName);
                        
                        if (pi.PropertyType.Name.ToLower() == "string")
                        {
                            pi.SetValue(theObject, bx.Text, null);
                        }
                        else if (pi.PropertyType.Name.ToLower() == "int32")
                        {
                            string temp = bx.Text.Replace(",", ".");//;
                            pi.SetValue(theObject, Convert.ToInt32(temp), null);
                        }
                        else if (pi.PropertyType.Name.ToLower() == "double")
                        {
                            string temp = bx.Text.Replace(".", ",");
                            pi.SetValue(theObject, Convert.ToDouble(temp), null);
                        }
                        else if (pi.PropertyType.Name.ToLower() == "float")
                        {
                            string temp = bx.Text.Replace(".", ",");
                            pi.SetValue(theObject, Convert.ToDouble(temp), null);
                        }
                        
                    }
                }
            }
        }





    }
}
