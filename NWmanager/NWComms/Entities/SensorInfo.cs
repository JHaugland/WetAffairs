using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class SensorInfo
    {
        #region "Constructors"

        public SensorInfo()
        {

        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        //public string Name { get; set; }

        public string SensorClassId { get; set; }

        public bool IsOperational { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeployedIntermediateDepth { get; set; }

        public string OwnerUnitId { get; set; }

        //public string OwnerPlayerId { get; set; }

        public double ReadyInSec { get; set; }

        public bool IsDamaged { get; set; }

        public GameConstants.SensorType SensorType { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            return string.Format("Sensor [{0}] {1}. Active: {2}", Id, SensorClassId, IsActive);
        }

        #endregion


    }
}
