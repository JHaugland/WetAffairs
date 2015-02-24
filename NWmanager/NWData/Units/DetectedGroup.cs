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
    public class DetectedGroup: GameObject
    {
        #region "Constructors"

        public DetectedGroup() : base()
        {
            DetectedUnits = new List<DetectedUnit>();
            DirtySetting = GameConstants.DirtyStatus.NewlyCreated;
        }

        #endregion


        #region "Public properties"

        public List<DetectedUnit> DetectedUnits { get; set; }

        public GameConstants.DirtyStatus DirtySetting { get; set; }

        public Position Position
        {
            get
            {
                if (DetectedUnits.Count > 0)
                {
                    return DetectedUnits[0].Position;
                }
                else
                {
                    return null;
                }
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name) && DetectedUnits.Count > 0)
                {
                    base.Name = DetectedUnits[0].Name + " group";
                }
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        public string GroupType 
        {
            get
            {
                if (DetectedUnits.Count > 0)
                {
                    return DetectedUnits[0].DetectionClassification.ToString();
                }
                else
                {
                    return "Empty";
                }
            }
        }
        
        #endregion



        #region "Public methods"

        public void SetDirty(GameConstants.DirtyStatus newDirtySetting)
        {
            if (newDirtySetting == GameConstants.DirtyStatus.Clean) //when set clean we mean it
            {
                DirtySetting = GameConstants.DirtyStatus.Clean;
            }
            else if ((int)newDirtySetting > (int)DirtySetting) //only change dirty setting if it increases dirtyness
            {
                DirtySetting = newDirtySetting;
            }
        }

        public bool RemoveUnit(DetectedUnit detectedUnit)
        {
            bool result = DetectedUnits.Remove(detectedUnit);
            if (result)
            {
                detectedUnit.DetectedGroupId = string.Empty;
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
            }
            return result;
        }

        public void AddUnit(DetectedUnit detectedUnit)
        {
            if (!DetectedUnits.Exists(u => u.Id == detectedUnit.Id))
            {
                DetectedUnits.Add(detectedUnit);
                detectedUnit.DetectedGroupId = this.Id;
                SetDirty(GameConstants.DirtyStatus.UnitChanged);
            }
        }

        public DetectedGroupInfo GetDetectedGroupInfo()
        {
            DetectedGroupInfo info = new DetectedGroupInfo();
            info.Id = Id;
            info.Name = Name;
            info.Description = ToString();
            foreach (var unit in DetectedUnits)
            {
                info.DetectedUnitIds.Add(unit.Id);
            }
            if (Position != null)
            {
                info.Position = Position.GetPositionInfo();
                info.Position.IsDetection = true;
            }
            return info;
        }

        public override string ToString()
        {
            return string.Format("{0} group [{1}] {2} ({3} units)", 
                GroupType, Id, Name, DetectedUnits.Count);
        }
        #endregion


    }
}
