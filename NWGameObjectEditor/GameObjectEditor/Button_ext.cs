using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;

namespace GameObjectEditor
{
    partial class ButtonExt : Button
    {
        public string PropertyName { get; set; }
        public object ObjectToModyfy { get; set; }
        public Type EditingType { get; set; }
    }
}
