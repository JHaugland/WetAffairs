using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class GroupInfo: IMarshallable
    {
        #region "Constructors"

        public GroupInfo()
        {

        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public string Name { get; set; }

        public string MainUnitId { get; set; }

        public string FormationId { get; set; }

        //public Formation Formation { get; set; }

        //public GameConstants.MissionType MissionType { get; set; }

        //public GameConstants.MissionTargetType MissionTargetType { get; set; }

        public int UnitCount { get; set; }

        public double MaxWeaponRangeAirM { get; set; }

        public double MaxWeaponRangeSurfaceM { get; set; }

        public double MaxWeaponRangeLandM { get; set; }

        public double MaxWeaponRangeSubM { get; set; }

        //public Dictionary<string, int> UnitTypeBreakdown { get; set; }

        #endregion



        #region "Public methods"

        public override string ToString()
        {
            return Id + " " + Name + " (" + UnitCount + " units)";
        }
        public string ToLongString()
        {
            string temp = ToString() + "\n";
            //foreach (var unit in UnitTypeBreakdown)
            //{
            //    temp += unit.Key + ":  " + unit.Value + "\n";
            //}
            //temp += "Mission: " + MissionType + "(" + MissionTargetType + ")\n";
            return temp;
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.GroupInfo; }
        }

        #endregion
    }
}
