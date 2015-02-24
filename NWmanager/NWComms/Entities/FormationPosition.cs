using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class FormationPosition : IMarshallable
    {
        #region "Constructors"

        public FormationPosition()
        {

        }

        #endregion


        #region "Public properties"

        public string Id { get; set; }

        public Nullable<TTG.NavalWar.NWComms.GameConstants.Role> Role { get; set; }

        public Nullable<GameConstants.UnitType> UnitType { get; set; }

        public string AssignedUnitId { get; set; }

        public PositionOffset PositionOffset { get; set; }

        public bool AutomaticallyLaunch { get; set; }

        public bool IsAssigned
        {
            get
            {
                return !string.IsNullOrEmpty(AssignedUnitId);
            }
        }

        #endregion



        #region "Public methods"
        public FormationPosition Clone()
        {
            FormationPosition fPosNew = (FormationPosition)this.MemberwiseClone();
            if (PositionOffset != null)
            {
                fPosNew.PositionOffset = PositionOffset.Clone();
            }
            else
            {
                Logger log = new Logger();
                log.LogWarning(
                    string.Format("FormationPosition {0} has PositionOffset null.", this.Id));
            }
            return fPosNew;
        }

        public override string ToString()
        {
            //return base.ToString();
            string temp = "FormationPostion " + this.Id + ". Assigned unit id: " + this.AssignedUnitId + ": ";
            if (this.PositionOffset != null)
            {
                temp += this.PositionOffset.ToString();
            }
            return temp;
        }

        #endregion



        #region IMarshallable Members

        public CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.FormationPosition; }
        }

        #endregion
    }
}
