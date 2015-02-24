using System;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;

namespace TTG.NavalWar.NWData
{
    [Serializable]
    public class BaseMovableObject : GameObject
    {
        #region "Constructors"

        private double? _DesiredBearingDeg = null;
        private double? _DesiredHeightOverSeaLevelM = 0;

        private Position _position;

        public BaseMovableObject() : base()
        {
            _position = new Position();
        }

        //public BaseMovableObject(bool doRegister)
        //    : this()
        //{
        //    if (doRegister)
        //    {
        //        //this.Register(); //TODO: Autoregistration not a good thing
        //    }
        //}
        #endregion

        #region "Public properties"

        /// <summary>
        /// Tag is used by the AI/scenarios to give units and groups a unique identifier.
        /// </summary>
        public string Tag { get; set; }

        public virtual Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public double? DesiredBearingDeg
        {
            get
            {
                return _DesiredBearingDeg;
            }
            set
            {
                _DesiredBearingDeg = value;
                if (ActualBearingDeg == null)
                {
                    ActualBearingDeg = _DesiredBearingDeg;
                }
            }
        }

        public double? ActualBearingDeg
        {
            get
            {
                if(Position != null)
                {
                    return Position.BearingDeg;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Position != null)
                {
                    if (value != null)
                    {
                        double val = (double)value;
                        if (val < 0)
                        {
                            val = val.ToRadian().ToDegreeBearing();
                        }
                        if (val > 360)
                        {
                            val = (val + 360) % 360;
                        }
                        Position.BearingDeg = val;
                    }
                    else
                    {
                        Position.BearingDeg = value;
                    }
                }
            }
        }

        public double? ActualHeightOverSeaLevelM
        {
            get
            {
                return Position.HeightOverSeaLevelM; 
            }
            set
            {
                Position.HeightOverSeaLevelM = value;
            }
        }

        public double? DesiredHeightOverSeaLevelM
        {
            get
            {
                return _DesiredHeightOverSeaLevelM;
            }
            set
            {
                _DesiredHeightOverSeaLevelM = value;
                if (Position.HeightOverSeaLevelM == null)
                {
                    Position.HeightOverSeaLevelM = value;
                }
            }
        }

        #endregion
    }
}
