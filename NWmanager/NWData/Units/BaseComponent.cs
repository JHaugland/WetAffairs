using System;
using TTG.NavalWar.NWData.GamePlay;

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class BaseComponent : GameObject
    {
        public BaseComponent() : base()
        {

        }

        #region "Public properties"

        public BaseUnit OwnerUnit { get; set; }

        public Player OwnerPlayer { get; set; }

        public double ReadyInSec { get; set; }

        public bool IsReady
        {
            get
            {
                if (!base.IsOperational || ReadyInSec > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }
        #endregion

        #region "Public methods"
        
        public virtual void Tick(double deltaGameTimeSec)
        {
            //Debug.WriteLine("ComponentBase->Tick for component " + this.Name);
            if (ReadyInSec > 0)
            {
                ReadyInSec -= deltaGameTimeSec;
            }
            if (ReadyInSec < 0)
            {
                ReadyInSec = 0;
            }
        }
        
        #endregion
    }
}
