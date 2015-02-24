using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace MemoryMapping
{
 
    [Flags]
    public enum MapProtection
    {
        PageNone = 0x00000000,
        // protection - mutually exclusive, do not or
        PageReadOnly = 0x00000002,
        PageReadWrite = 0x00000004,
        PageWriteCopy = 0x00000008,
        // attributes - or-able with protection
        SecImage = 0x01000000,
        SecReserve = 0x04000000,
        SecCommit = 0x08000000,
        SecNoCache = 0x10000000,
    }


    public enum MapAccess
    {
        FileMapCopy = 0x0001,
        FileMapWrite = 0x0002,
        FileMapRead = 0x0004,
        FileMapAllAccess = 0x001f,
    }

    public class MemoryMappedFile : MarshalByRefObject, IDisposable
    {
        
        private IntPtr _hMap = IntPtr.Zero;
        private MapProtection _protection = MapProtection.PageNone;

        private string _fileName = "";
        public string FileName { get { return _fileName; } }

        private long _maxSize;
        private readonly bool _is64bit;

        public long MaxSize { get { return _maxSize; } }

        #region Constants

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = unchecked((int)0x40000000);
        private const int OPEN_ALWAYS = 4;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        private static readonly IntPtr NULL_HANDLE = IntPtr.Zero;

        #endregion

        #region Properties
        public bool IsOpen
        {
            get { return (_hMap != NULL_HANDLE); }
        }

        public bool Is64bit
        {
            get { return _is64bit; }
        }
        #endregion


        private MemoryMappedFile()
        {
            _is64bit = IntPtr.Size == 8;
        }

        ~MemoryMappedFile()
        {
            Dispose(false);
        }

        #region Create Overloads


        public static MemoryMappedFile
            Create(MapProtection protection, long maxSize, string name)
        {
            return Create(null, protection, maxSize, name);
        }

        public static MemoryMappedFile
            Create(MapProtection protection, long maxSize)
        {
            return Create(null, protection, maxSize, null);
        }

        public static MemoryMappedFile
            Create(string fileName, MapProtection protection)
        {
            return Create(fileName, protection, 0, null);
        }


        public static MemoryMappedFile
            Create(string fileName, MapProtection protection,
                              long maxSize)
        {
            return Create(fileName, protection, maxSize, null);
        }

        public static MemoryMappedFile
            Create(string fileName, MapProtection protection,
                              long maxSize, String name)
        {
            MemoryMappedFile map = new MemoryMappedFile();
            if (!map.Is64bit && maxSize > uint.MaxValue)
                throw new ConstraintException("32bit systems support max size of 4gb.");

            // open file first
            IntPtr hFile = INVALID_HANDLE_VALUE;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (maxSize == 0)
                {
                    if (!File.Exists(fileName))
                    {
                        throw new MMException(string.Format("MemoryMappedFile.Create - \"{0}\" does not exist ==> Unable to map entire file", fileName));
                    }

                    FileInfo backingFileInfo = new FileInfo(fileName);
                    maxSize = backingFileInfo.Length;

                    if (maxSize == 0)
                    {
                        throw new MMException(string.Format("Create - \"{0}\" is zero bytes ==> Unable to map entire file", fileName));
                    }
                }

               
                int desiredAccess = GENERIC_READ;
                if ((protection == MapProtection.PageReadWrite) ||
                      (protection == MapProtection.PageWriteCopy))
                {
                    desiredAccess |= GENERIC_WRITE;
                }

                hFile = Win32MapApis.CreateFile(
                            fileName, desiredAccess, 0,
                            IntPtr.Zero, OPEN_ALWAYS, 0, IntPtr.Zero
                          );
                if (hFile == INVALID_HANDLE_VALUE)
                    throw new MMException(Marshal.GetHRForLastWin32Error());

                map._fileName = fileName;
            }

            map._hMap = Win32MapApis.CreateFileMapping(
                        hFile, IntPtr.Zero, (int)protection,
                        (int)((maxSize >> 32) & 0xFFFFFFFF),
                        (int)(maxSize & 0xFFFFFFFF), name
                    );

           
            if (hFile != INVALID_HANDLE_VALUE) Win32MapApis.CloseHandle(hFile);
            if (map._hMap == NULL_HANDLE)
                throw new MMException(Marshal.GetHRForLastWin32Error());

            map._protection = protection;
            map._maxSize = maxSize;

            return map;
        }

        #endregion 

        
        public static MemoryMappedFile Open(MapAccess access, String name)
        {
            MemoryMappedFile map = new MemoryMappedFile
            {
                _hMap = Win32MapApis.OpenFileMapping((int)access, false, name)
            };

            if (map._hMap == NULL_HANDLE)
                throw new MMException(Marshal.GetHRForLastWin32Error());
            map._maxSize = -1; 
            return map;
        }

       
        public void Close()
        {
            Dispose(true);
        }

        public IntPtr MapView(MapAccess access, long offset, long size)
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Memmoryfile Already closed");

           
            IntPtr mapSize = new IntPtr(size);

            IntPtr baseAddress = Win32MapApis.MapViewOfFile(
              _hMap, (int)access,
              (int)((offset >> 32) & 0xFFFFFFFF),
              (int)(offset & 0xFFFFFFFF), mapSize
              );

            if (baseAddress == IntPtr.Zero)
                throw new MMException(Marshal.GetHRForLastWin32Error());

            return baseAddress;

        }

        public MapViewStream MapAsStream()
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Memmoryfile Already closed");

            bool isWriteable = (_protection & MapProtection.PageReadWrite) == MapProtection.PageReadWrite;
            return new MapViewStream(this, MaxSize, isWriteable);

        }

        public void UnMapView(IntPtr mapBaseAddr)
        {
            Win32MapApis.UnmapViewOfFile(mapBaseAddr);
        }

        public void UnMapView(MapViewStream mappedViewStream)
        {
            UnMapView(mappedViewStream.ViewBaseAddr);
        }

        public void Flush(IntPtr viewBaseAddr)
        {
            
            IntPtr flushLength = new IntPtr(MaxSize);
            Win32MapApis.FlushViewOfFile(viewBaseAddr, flushLength);
        }

        public void Flush(MapViewStream mappedViewStream)
        {
            Flush(mappedViewStream.ViewBaseAddr);
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsOpen)
                Win32MapApis.CloseHandle(_hMap);
            _hMap = NULL_HANDLE;

            if (disposing)
                GC.SuppressFinalize(this);
        }

        #endregion 

    }
}
