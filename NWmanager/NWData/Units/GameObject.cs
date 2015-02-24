using System;

namespace TTG.NavalWar.NWData
{
    /// <summary>
    /// All objects that should be registered (automatic ID) in the game should inherit from GameObject.
    /// Objects where all instances are loaded from xml (for example UnitClass or WeaponClass) should not. 
    /// </summary>
    [Serializable]
    public class GameObject
    {
        private bool _isMarkedForDeletion = false;
        
        #region "Constructors"

        public GameObject()
        {
            Id = GameManager.GetUniqueCode();
            IsOperational = true;
        }

        #endregion

        #region "Public properties"

        public string Id { get; set; }

        public bool IsOperational { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsMarkedForDeletion
        {
            get
            {
                return _isMarkedForDeletion;
            }
            set
            {
                _isMarkedForDeletion = value;
            }

        }
        #endregion
    }
}
