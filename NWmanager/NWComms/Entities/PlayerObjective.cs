using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using System.Xml.Serialization;

namespace TTG.NavalWar.NWComms.Entities
{
    [Serializable]
    public class PlayerObjective : EventTrigger
    {
        #region "Constructors"

        public PlayerObjective()
        {
        }

        #endregion

        #region "Public properties"

        public override bool IsPlayerObjective
        {
            get
            {
                return true;
            }
        }

        public bool IsVictoryCondition { get; set; }

        /// <summary>
        /// If true, this means that reaching this victory condition alone is sufficient for victory. Has no meaning if
        /// IsVictoryCondition is false.
        /// </summary>
        public bool IsExclusiveVictoryCondition { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool IsPassiveObjective { get; set; }

        //public string Description { get; set; }

        //public EventTriggerInfo EventTrigger { get; set; }

        //public DefeatConditionInfo DefeatCondition { get; set; }

        //public int CountToMeet { get; set; }

        //public int CountCurrent { get; set; }

        //public bool HasBeenMet { get; set; }

        #endregion



        #region "Public methods"

        //public override EventTrigger Clone()
        //{
        //    return (EventTrigger)this.MemberwiseClone();
        //}

        public override string ToString()
        {
            var temp = "PlayerObjective " + base.ToString();
            if (IsVictoryCondition)
            {
                temp += " Victory Condition";
            }
            return temp;
        }

        public override bool Equals( System.Object obj )
        {
            // If parameter is null return false.
            if ( obj == null )
            {
                return false;
            }

            // If parameter cannot be cast to PlayerObjective return false.
            PlayerObjective p = obj as PlayerObjective;
            if ( ( System.Object )p == null )
            {
                return false;
            }

            // Return true if the fields match:
            return (this.DescriptionPlayer == p.DescriptionPlayer) && 
                ( CountDesired == p.CountDesired) && 
                (CountCurrent == p.CountCurrent) && 
                (HasBeenTriggered == p.HasBeenTriggered);
        }

        public bool Equals( PlayerObjective p )
        {
            // If parameter is null return false:
            if ( ( object )p == null )
            {
                return false;
            }

            // Return true if the fields match:
            return ( DescriptionPlayer == p.DescriptionPlayer ) &&
                ( CountDesired == p.CountDesired) &&
                ( CountCurrent == p.CountCurrent ) &&
                ( HasBeenTriggered == p.HasBeenTriggered);
        }

        #endregion

        public override CommsMarshaller.ObjectTokens ObjectTypeToken
        {
            get { return CommsMarshaller.ObjectTokens.PlayerObjective; }
        }



    }
}
