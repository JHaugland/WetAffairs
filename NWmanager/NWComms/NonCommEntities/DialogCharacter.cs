using System;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class DialogCharacter : IGameDataObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public Color TextColor { get; set; }
    }
}
