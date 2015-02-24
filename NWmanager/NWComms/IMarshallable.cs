using System;
using System.Collections.Generic;
using System.Text;

namespace TTG.NavalWar.NWComms
{
    public interface IMarshallable
    {
        CommsMarshaller.ObjectTokens ObjectTypeToken { get; }
    }
}
