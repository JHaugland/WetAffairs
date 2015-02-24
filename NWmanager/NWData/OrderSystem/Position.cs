using System;
using System.Collections.Generic;
using System.Linq;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.OrderSystem
{
    [Serializable]
    public class Position
    {
        private Coordinate _coordinate;
        private const string COORD_FORMAT = "{0:F}";

        #region "Constructors"

        public Position()
        {
            _coordinate = new Coordinate();
        }

        public Position(Coordinate coordinate)
            : this()
        {
            _coordinate = coordinate.Clone();
            HeightOverSeaLevelM = null;
        }

        public Position(PositionInfo positionInfo)
            : this()
        {
            _coordinate = new Coordinate(positionInfo.Latitude, positionInfo.Longitude);
            if (positionInfo.HasHeightOverSeaLevel)
            {
                HeightOverSeaLevelM = positionInfo.HeightOverSeaLevelM;
            }
            else
            {
                HeightOverSeaLevelM = null;
            }
            BearingDeg = positionInfo.BearingDeg;

        }

        public Position(double latittude, double longitude): this()
        {
            _coordinate.LatitudeDeg = latittude;
            _coordinate.LongitudeDeg = longitude;
            HeightOverSeaLevelM = null;
            BearingDeg = null;
        }

        public Position(double latittude, double longitude, double heightOverSeaLevelM)
            : this()
        {
            _coordinate.LatitudeDeg = latittude;
            _coordinate.LongitudeDeg = longitude;
            HeightOverSeaLevelM = heightOverSeaLevelM;
            BearingDeg = null;
        }

        public Position(double latittude, double longitude, double heightOverSeaLevelM, double bearingDeg)
            : this()
        {
            _coordinate.LatitudeDeg = latittude;
            _coordinate.LongitudeDeg = longitude;
            HeightOverSeaLevelM = heightOverSeaLevelM;
            BearingDeg = bearingDeg;
        }


        public Position(string coordinateString, double heightOverSeaLevelM)
            : this()
        {
            _coordinate = Coordinate.ParseFromString(coordinateString);
            HeightOverSeaLevelM = heightOverSeaLevelM;
        }

        public Position(string coordinateString, double heightOverSeaLevelM, double bearingDeg)
            : this()
        {
            _coordinate = Coordinate.ParseFromString(coordinateString);
            HeightOverSeaLevelM = heightOverSeaLevelM;
            BearingDeg = bearingDeg;
        }


        #endregion

        #region "Public properties"

        /// <summary>
        /// Coordinate of position as expressed in latitude and longitude
        /// </summary>
        public Coordinate Coordinate 
        {
            get
            {
                return _coordinate;
            }
            set
            {
                _coordinate = value;
            }
        }

        public bool HasHeightOverSeaLevel
        {
            get
            {
                return HeightOverSeaLevelM.HasValue;

            }

        }
        public double? HeightOverSeaLevelM { get; set; }

        public bool HasBearing
        {
            get
            {
                return BearingDeg.HasValue;
            }
        }

        public double? BearingDeg { get; set; }

        //public bool HasPitchAngle
        //{
        //    get
        //    {
        //        return PitchAngleDeg.HasValue;
        //    }
        //}
        ///// <summary>
        ///// Sets the angle of pitch for the unit, mostly relevant for aircraft and missiles. 0 is defined as 
        ///// the forward direction of the unit.
        ///// </summary>
        //public double? PitchAngleDeg { get; set; } //like bearing, but vertically

        #endregion

        #region "Public methods"
        
        public Position Clone()
        {
            Position NewPosition = new Position(this.Coordinate.LatitudeDeg, this.Coordinate.LongitudeDeg);
            if (this.HasHeightOverSeaLevel)
            {
                NewPosition.HeightOverSeaLevelM = this.HeightOverSeaLevelM;
            }
            if (this.HasBearing)
            {
                NewPosition.BearingDeg = this.BearingDeg;
            }
            return NewPosition;
        }

        public Position Offset(PositionOffset offset)
        {
            Position pos = MapHelper.CalculatePositionFromOffset2(this, offset);

            return pos;

        }

        public Position Offset(FormationPosition formPosition)
        {
            Position pos = MapHelper.CalculatePositionFromOffset2(this, formPosition.PositionOffset);

            return pos;
        }

        public Position Offset(double bearingDeg, double distanceM)
        {
            Coordinate coord = MapHelper.CalculateNewPosition2(this.Coordinate, bearingDeg, distanceM);
            var newPos = new Position(coord);
            if (this.HasBearing)
            {
                newPos.BearingDeg = this.BearingDeg;
            }
            if (this.HasHeightOverSeaLevel)
            {
                newPos.HeightOverSeaLevelM = this.HeightOverSeaLevelM;
            }
            return newPos;
        }

        public Position IgnoreElevation()
        {
            var pos = this.Clone();
            pos.HeightOverSeaLevelM = null;
            return pos;
        }

        public void SetNewCoordinate(double latitude, double longitude)
        {
            
            this.Coordinate = new Coordinate(latitude, longitude);
            //_coordinate.Latitude = latitude;
            //_coordinate.Longitude = longitude;
        }

        public void SetNewCoordinate(Coordinate coordinate)
        {
            this.Coordinate = coordinate.Clone();
        }

        public void SetNewBearing(double bearingDeg)
        {
            BearingDeg = bearingDeg;
        }

        public override bool Equals(Object obj)
        {
            Position pos = obj as Position;
            if (pos == null)
            {
                return false;
            }
            if (pos.Coordinate == null)
            {
                return false;
            }
            if (pos.Coordinate.LatitudeDeg != Coordinate.LatitudeDeg || pos.Coordinate.LongitudeDeg != Coordinate.LongitudeDeg)
            {
                return false;
            }
            if (pos.HeightOverSeaLevelM != HeightOverSeaLevelM)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string temp = Coordinate.ToString();
            //"Lat:" + FormatDouble(Coordinate.Latitude) + " Lon:" + FormatDouble(Coordinate.Longitude);
            if (HasBearing)
            {
                temp += " Bearing:" + FormatDouble(BearingDeg);
            }
            if (HasHeightOverSeaLevel)
            {
                temp += " Height:" + FormatDouble(HeightOverSeaLevelM);
            }
            return temp;
        }

        public PositionInfo GetPositionInfo()
        {
            PositionInfo pos = new PositionInfo();
            pos.Latitude = this.Coordinate.LatitudeDeg;
            pos.Longitude = this.Coordinate.LongitudeDeg;
            ProjCoordinate projOrthoCoord = this.Coordinate.ToOrthoProjectedCoordinate();
            pos.LatitudeOrthoProjected = projOrthoCoord.Latitude;
            pos.LongitudeOrthoProjected = projOrthoCoord.Longitude;
            pos.IsDetection = false;
            if (this.HasBearing)
            {
                pos.BearingDeg = (float)this.BearingDeg;
                //pos.Bearing = pos.BearingDeg.ToCardinalMark();
            }            
            if (this.HasHeightOverSeaLevel)
            {
                pos.HeightOverSeaLevelM = (float)this.HeightOverSeaLevelM;
            }
            
            return pos;
        }
        #endregion

        #region "Private methods"

        private string FormatDouble(double? value)
        {
            if (value.HasValue)
            {
                return string.Format(COORD_FORMAT, value);
            }
            else
            {
                return "(null)";
            }

        }
        #endregion
    }
}
