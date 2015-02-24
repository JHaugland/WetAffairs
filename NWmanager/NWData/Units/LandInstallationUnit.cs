using System;
using TTG.NavalWar.NWComms;

namespace TTG.NavalWar.NWData.Units
{
    [Serializable]
    public class LandInstallationUnit : BaseUnit
    {
        #region "Constructors"

        public LandInstallationUnit() : base()
        {

        }

        #endregion

        #region "Public properties"

        public override GameConstants.MissionType MissionType
        {
            get
            {
                return  GameConstants.MissionType.Other;
            }
            set
            {
                base.MissionType = value;
            }
        }

        public override GameConstants.MissionTargetType MissionTargetType
        {
            get
            {
                return  GameConstants.MissionTargetType.Undefined;
            }
            set
            {
                base.MissionTargetType = value;
            }
        }

        public override DetectedUnit TargetDetectedUnit
        {
            get
            {
                return null;
            }
            set
            {
                base.TargetDetectedUnit = value;
            }
        }

        #endregion

        #region "Public methods"

        public override void MoveToNewCoordinate(double gameTime)
        {
            return; // No movement to perform
            //base.MoveToNewCoordinate(gameTime);
        }
        
        #endregion
    }
}
