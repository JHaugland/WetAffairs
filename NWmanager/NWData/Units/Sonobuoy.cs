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

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class Sonobuoy : BaseUnit
    {
        #region "Constructors"

        public Sonobuoy() : base()
        {

        }

        #endregion


        #region "Public properties"

        #endregion



        #region "Public methods"

        public override void MoveToNewPosition3D(double elapsedGameTimeSec)
        {
            if (DesiredHeightOverSeaLevelM > UnitClass.HighestOperatingHeightM)
            {
                DesiredHeightOverSeaLevelM = UnitClass.HighestOperatingHeightM;
                SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
            }
            if (DesiredHeightOverSeaLevelM < UnitClass.LowestOperatingHeightM)
            {
                DesiredHeightOverSeaLevelM = UnitClass.LowestOperatingHeightM;
                SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
            }
            double changeM = UpdateElevation(elapsedGameTimeSec, 20);
            if (changeM > 0)
            {
                SetDirty(GameConstants.DirtyStatus.PositionOnlyChanged);
            }
            return; //TODO: Maybe drift a bit with the wind/tide??
            //base.MoveToNewPosition3D(elapsedGameTimeSec);
        }

        #endregion


    }
}
