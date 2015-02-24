using System;
using System.IO;

namespace TTG.NavalWar.NWData.Util.MemoryMapping
{
    [Serializable]
    public class MMException : IOException
    {
        public int Win32ErrorCode { get; private set; }

        public override string Message
        {
            get
            {
                if (Win32ErrorCode != 0)
                    return base.Message + " (" + Win32ErrorCode + ")";

                return base.Message;
            }
        }

        public MMException(int error)
            : base()
        {
            Win32ErrorCode = error;
        }

        public MMException(string message)
            : base(message)
        {
        }

        public MMException(string message, MMException innerException)
            : base(message, innerException)
        {
        }
    }
}
