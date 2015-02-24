using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace GameObjectEditor
{
    class TextboxExt : TextBox
    {
        public string PropertyName { get; set; }
        public object ObjectToModyfy { get; set; }
        
    }
}
