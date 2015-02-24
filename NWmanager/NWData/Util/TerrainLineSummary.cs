using System;

namespace TTG.NavalWar.NWData.Util
{
    [Serializable]
    public class TerrainLineSummary 
    {
        #region "Constructors"

        public TerrainLineSummary()
        {

        }

        public TerrainLineSummary(int maxHeightM, int minHeightM, int heightVarianceM)
        {
            MaxHeightM = maxHeightM;
            MinHeightM = minHeightM;
            HeightVarianceM = heightVarianceM;
        }

        #endregion

        #region "Public properties"

        public int MaxHeightM { get; set; }

        public int MinHeightM { get; set; }

        public int HeightVarianceM { get; set; }

        public int MaxHeightBehindM { get; set; }

        public int MinHeightBehindM { get; set; }

        public int HeightVarianceBehindM { get; set; }

        #endregion
    }
}
