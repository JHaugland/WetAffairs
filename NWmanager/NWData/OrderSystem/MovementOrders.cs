using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable][Obsolete]
    public class MovementOrders : Orders<MovementOrder>
    {
        private MovementOrder _ActiveMovementOrder = null;

        #region "Constructors"

        public MovementOrders() :base()
        {
        }

        #endregion


        #region "Public properties"
        
        public virtual MovementOrder ActiveMovementOrder 
        { 
            get
            {
                return _ActiveMovementOrder;
            }
            set
            {
                _ActiveMovementOrder = value;
            }
        }

        #endregion



        #region "Public methods"

        public virtual void ActivateNextMovementOrder()
        {
            if (_ActiveMovementOrder != null)
            {
                this.Remove(_ActiveMovementOrder);
            }
            _ActiveMovementOrder = DequeueNextMovementOrder();
            
        }

        public virtual MovementOrder DequeueNextMovementOrder()
        {
            return base.Dequeue();
        }
        #endregion


    }
}
