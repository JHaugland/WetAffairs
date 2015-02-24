using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TTG.NavalWar.NWData.Util
{
    [Serializable]
    public class FlexQueue<T> : LinkedList<T>
    {
        public FlexQueue()
            : base()
        {
            
        }

        public FlexQueue(IEnumerable<T> collection)
            : base(collection)
        {
            
        }

        public void Enqueue( T item )
        {
            AddLast(item);
        }

        public T Dequeue()
        {
            T item = First.Value;
            RemoveFirst();
            return item;
        }

        public T Peek()
        {
            return First.Value;
        }
    }
}
