using System;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class Vector2
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vector2()
        {

        }

        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public override string ToString()
        {
            return x + "," + y;
        }
    }
}
