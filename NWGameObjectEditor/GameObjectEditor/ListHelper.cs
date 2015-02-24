using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameObjectEditor
{
    public class ListHelper<T>
    {
        public List<T> theList;
        public ListHelper(List<T> anyList)
        {
            theList = anyList;
        }
        public ListHelper()
        {
           
        }

    }
}
