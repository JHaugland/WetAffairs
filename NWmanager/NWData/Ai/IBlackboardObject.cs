using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Ai
{
    public interface IBlackboardObject
    {
        string Id { get; set; }
        string OwnerId { get; set; }
        Coordinate Coordinate { get; set; }

        double DistanceToM(Coordinate coordinate);
    }
}
