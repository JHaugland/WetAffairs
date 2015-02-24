using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class DetecedUnitSensorInfo :IMarshallable
    {
        #region "Constructors"

        public DetecedUnitSensorInfo()
        {

        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string SensorId { get; set; }

        public string PlatformId { get; set; }

        public string SensorName { get; set; }

        public string SensorDescription { get; set; }

        public string PlatformName { get; set; }

        public double BearingToTargetDeg { get; set; }

        public double DistanceToTargetM { get; set; }

        public double DetectedGameWorldTimeSec { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            return string.Format("Sensor {0} on unit {1}: Bearing {2:F}deg, Distance {3:F}m",
                SensorDescription, PlatformName, BearingToTargetDeg, DistanceToTargetM);
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.DetecedUnitSensorInfo; }
        }

        #endregion
    }
}
