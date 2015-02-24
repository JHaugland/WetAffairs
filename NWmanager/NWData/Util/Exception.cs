using System;
using System.IO;

namespace MemoryMapping
{
    [Serializable]
    public class MMException : IOException
    {

        private int m_win32Error;
        public int Win32ErrorCode
        {
            get { return m_win32Error; }
        }
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
            m_win32Error = error;
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
