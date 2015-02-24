using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;
using NWmanager;

namespace NWmanagerLib
{
    public static class NWManager
    {
        public static void CreateData()
        {
            UnitDatabaseFactory.CreateUnitDatabase( null );
            
        }

        public static void CreateData( string path )
        {
            SerializationHelper.SetDataFolder( path );
            UnitDatabaseFactory.CreateUnitDatabase( null );
        }
    }
}
