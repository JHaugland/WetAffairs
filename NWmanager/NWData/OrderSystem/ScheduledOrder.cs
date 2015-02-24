using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class ScheduledOrder : BaseOrder
    {
        #region "Constructors"

        public ScheduledOrder() : base()
        {
            RecurringCount = 1;
            OrderType = GameConstants.OrderType.ScheduledOrder;
        }

        public ScheduledOrder(double triggerInSec) : this()
        {
            TriggerInSec = triggerInSec;
        }

        public ScheduledOrder(double triggerInSec, int recurringCount)
            : this()
        {
            RecurringCount = recurringCount;
            TriggerInSec = triggerInSec;
        }

        #endregion


        #region "Public properties"

        private double _TriggerInSec;
        public virtual double TriggerInSec
        {
            get
            {
                return _TriggerInSec;
            }
            set
            {
                _TriggerInSec = value;
                if (_TriggerInSec < 0)
                {
                    _TriggerInSec = 0;
                }
                if (GameManager.Instance.Game != null)
                {
                    DateTime currentTime = GameManager.Instance.Game.GameCurrentTime;
                    
                    TriggerNextTime = currentTime.AddSeconds(_TriggerInSec);
                }
                
            }
        }

        public virtual DateTime TriggerNextTime { get; set; }

        public virtual int RecurringCount { get; set; }

        #endregion



        #region "Public methods"

        #endregion


    }
}
