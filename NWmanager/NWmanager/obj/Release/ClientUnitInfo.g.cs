﻿#pragma checksum "..\..\ClientUnitInfo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "810E6A61622AA973BDCB6600FC933531"
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
using System.Windows.Forms.Integration;
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


namespace NWmanager {
    
    
    /// <summary>
    /// ClientUnitInfo
    /// </summary>
    public partial class ClientUnitInfo : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\ClientUnitInfo.xaml"
        internal System.Windows.Controls.StackPanel stackPanel1;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\ClientUnitInfo.xaml"
        internal System.Windows.Controls.Label lblUnit;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\ClientUnitInfo.xaml"
        internal System.Windows.Controls.TextBox txtUnitInfo;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\ClientUnitInfo.xaml"
        internal System.Windows.Controls.Label lblDet;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\ClientUnitInfo.xaml"
        internal System.Windows.Controls.TextBox txtDetectedUnitInfo;
        
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
            System.Uri resourceLocater = new System.Uri("/NWmanager;component/clientunitinfo.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ClientUnitInfo.xaml"
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
            this.stackPanel1 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.lblUnit = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.txtUnitInfo = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.lblDet = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.txtDetectedUnitInfo = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

