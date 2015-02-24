using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTG.NavalWar.NWComms.NonCommEntities
{
    [Serializable]
    public class Color
    {
        public Color()
        {

        }

        public Color( float r, float g, float b )
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color( int r, int g, int b )
        {
            this.r = r / 255f;
            this.g = g / 255f;
            this.b = b / 255f;
        }

        public float r = 1f;
        public float g = 1f;
        public float b = 1f;
    }
}
