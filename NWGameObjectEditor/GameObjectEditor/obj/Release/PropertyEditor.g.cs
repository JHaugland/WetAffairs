﻿#pragma checksum "..\..\PropertyEditor.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2850BD92E95358E77A59D0EEC8D51D52"
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
    /// PropertyEditor
    /// </summary>
    public partial class PropertyEditor : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\PropertyEditor.xaml"
        internal System.Windows.Controls.Grid ContentGrid;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\PropertyEditor.xaml"
        internal System.Windows.Controls.ColumnDefinition label;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\PropertyEditor.xaml"
        internal System.Windows.Controls.ColumnDefinition valueField;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\PropertyEditor.xaml"
        internal System.Windows.Controls.ColumnDefinition btnField;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\PropertyEditor.xaml"
        internal System.Windows.Controls.Label lblName;
        
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
            System.Uri resourceLocater = new System.Uri("/GameObjectEditor;component/propertyeditor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PropertyEditor.xaml"
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
            this.ContentGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.label = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 3:
            this.valueField = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 4:
            this.btnField = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 5:
            this.lblName = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

