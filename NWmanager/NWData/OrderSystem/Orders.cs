using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class Orders<BaseOrder> : FlexQueue<BaseOrder> 
    {

        //private FlexQueue<BaseOrder> _Orders = null;

        #region "Constructors"

        public Orders() : base()
        {
            //_Orders = new FlexQueue<BaseOrder>();
            
        }

        #endregion


        #region "Public properties"

        #endregion



        #region "Public methods"

        public void Add(BaseOrder order)
        {
            this.Enqueue(order);
        }
        
        //public bool Remove(BaseOrder order)
        //{
        //    return _Orders.Remove(order);
        //}
        #endregion


        //#region IEnumerable<> Members

        //IEnumerator<BaseOrder> IEnumerable<BaseOrder>.GetEnumerator()
        //{
        //    return _Orders.GetEnumerator();
        //}

        //#endregion

        //#region IEnumerable Members

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return _Orders.GetEnumerator();
        //}

        //#endregion
    }
}
