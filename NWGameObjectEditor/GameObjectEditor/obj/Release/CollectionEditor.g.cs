﻿#pragma checksum "..\..\CollectionEditor.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "278C8DF43F3A2B2AA22DBD812236A552"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GameObjectEditor {
    
    
    /// <summary>
    /// CollectionEditor
    /// </summary>
    public partial class CollectionEditor : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.WrapPanel spnlControls;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.Button Save;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.Button Canel;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.ListBox lbxEnteties;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.ToolBar tbToolBar;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.Menu MainMenu;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\CollectionEditor.xaml"
        internal System.Windows.Controls.MenuItem mnuNewItem;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GameObjectEditor;component/collectioneditor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CollectionEditor.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.spnlControls = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 2:
            this.Save = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\CollectionEditor.xaml"
            this.Save.Click += new System.Windows.RoutedEventHandler(this.Save_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Canel = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\CollectionEditor.xaml"
            this.Canel.Click += new System.Windows.RoutedEventHandler(this.Canel_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lbxEnteties = ((System.Windows.Controls.ListBox)(target));
            
            #line 20 "..\..\CollectionEditor.xaml"
            this.lbxEnteties.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lbxEnteties_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.tbToolBar = ((System.Windows.Controls.ToolBar)(target));
            return;
            case 6:
            this.MainMenu = ((System.Windows.Controls.Menu)(target));
            
            #line 26 "..\..\CollectionEditor.xaml"
            this.MainMenu.AddHandler(System.Windows.Controls.MenuItem.ClickEvent, new System.Windows.RoutedEventHandler(this.Menu_Click));
            
            #line default
            #line hidden
            return;
            case 7:
            this.mnuNewItem = ((System.Windows.Controls.MenuItem)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

