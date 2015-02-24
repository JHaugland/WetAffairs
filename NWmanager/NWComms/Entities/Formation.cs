using System;
using System.Collections.Generic;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class Formation :IMarshallable, IGameDataObject
    {
        #region "Constructors"

        public Formation()
        {
            FormationPositions = new List<FormationPosition>();
        }

        #endregion

        #region "Public properties"

        public string Id { get; set; }

        public string Description { get; set; }

        public bool IsAircraftFormation { get; set; }

        public List<FormationPosition> FormationPositions { get; set; }

        #endregion

        #region "Public methods"

        public Formation Clone()
        {
            Formation newFormation = (Formation)this.MemberwiseClone();
            newFormation.FormationPositions = new List<FormationPosition>();
            foreach (var fpos in this.FormationPositions)
            {
                var fposNew = fpos.Clone();
                fposNew.AssignedUnitId = string.Empty;
                newFormation.FormationPositions.Add(fposNew);
            }
            return newFormation;
        }

        public FormationPosition GetFormationPositionById(string id)
        {
            try
            {
                return FormationPositions.Find(s => s.Id == id);
            }
            catch (Exception ex)
            {
                Logger log = new Logger();
                log.LogError(
                    "GetGameScenarioById failed for Id=" + id + ". " + ex.ToString());
                return null;
            }
        }
        public FormationPosition GetPositionForUnit(GameConstants.UnitType unitType, HashSet<GameConstants.Role> roleList)
        {
            try
            {
                foreach (var fp in FormationPositions)
                {
                    if (!fp.IsAssigned)
                    {
                        if (fp.UnitType != null && fp.UnitType == unitType)
                        {
                            return fp;
                        }
                        if (fp.Role != null && roleList.Contains( ( GameConstants.Role )fp.Role ))
                        {
                            return fp;
                        }
                        if (fp.Role == null && fp.UnitType == null)
                        {
                            return fp;
                        }
                    }
                }
                return null; //not found
            }
            catch (Exception ex)
            {
                Logger log = new Logger();
                log.LogError(
                    "GetPositionForUnit failed for UnitType=" + unitType + ". " + ex.ToString() );
                return null;
            }
        }

        /// <summary>
        /// If no position is available for unit, create a new position somewhat sensibly
        /// </summary>
        /// <param name="unitInfo"></param>
        /// <returns></returns>
        public FormationPosition CreateNewPositionForUnit(GameConstants.UnitType unitType)
        {
            FormationPosition fpos = new FormationPosition();
            double distanceM = 500;
            if (unitType == GameConstants.UnitType.FixedwingAircraft ||
                unitType == GameConstants.UnitType.Helicopter)
            {
                distanceM = 150;
            }
            //TODO: Make this more sensible
            distanceM = distanceM * this.FormationPositions.Count;
            fpos.PositionOffset = new PositionOffset(distanceM / 3, (-1 * distanceM) / 2, 0);
            return fpos;
        }

        public override string ToString()
        {
            string temp = string.Empty;
            if (!string.IsNullOrEmpty(Description))
            {
                temp = "Formation " + Id + " " + Description;
            }
            else
            {
                temp = "Formation " + Id;
            }
            temp += "\n";
            foreach (var fpos in this.FormationPositions)
            {
                temp += fpos.ToString() + "\n";
            }
            return temp;
        }

        #endregion

        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.Formation; }
        }

        #endregion
    }
}
