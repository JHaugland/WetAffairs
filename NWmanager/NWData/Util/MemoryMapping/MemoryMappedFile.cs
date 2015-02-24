using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace TTG.NavalWar.NWData.Util.MemoryMapping
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

    public class MemoryMappedFile : MarshalByRefObject, IDisposable
    {        
        private IntPtr _hMap = IntPtr.Zero;
        private MapProtection _protection = MapProtection.PageNone;

        private string _fileName = "";
        public string FileName { get { return _fileName; } }

        private long _fileSize;
        private long _maxSize;
        private readonly bool _is64bit;

        public long FileSize { get { return _fileSize; } }
        public long MaxSize { get { return _maxSize; } }

        #region Constants

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = unchecked((int)0x40000000);
        private const int OPEN_EXISTING = 3;
        private const int FILE_SHARE_READ = 0x00000001;
        private const int FILE_SHARE_WRITE = 0x00000002;
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
                if (!File.Exists(fileName))
                {
                    throw new MMException(string.Format( "MemoryMappedFile.Create - \"{0}\" does not exist ==> Unable to map file", fileName));
                }

                FileInfo backingFileInfo = new FileInfo(fileName);

                // Get the file size and store it for later use.
                map._fileSize = backingFileInfo.Length;

                // Set max size if not set. Otherwise cap it.
                if ( maxSize == 0 )
                    maxSize = backingFileInfo.Length;
                else
                    maxSize = Math.Min(maxSize, backingFileInfo.Length);

                if (maxSize == 0)
                {
                    throw new MMException(string.Format( "Create - \"{0}\" is zero bytes ==> Unable to map file", fileName));
                }
               
                int desiredAccess = GENERIC_READ;
                int desiredShareMode = FILE_SHARE_READ;
                if ((protection == MapProtection.PageReadWrite) ||
                      (protection == MapProtection.PageWriteCopy))
                {
                    desiredAccess |= GENERIC_WRITE;
                    desiredShareMode |= FILE_SHARE_WRITE;
                }

                hFile = Win32MapApis.CreateFile(
                            fileName, desiredAccess, desiredShareMode,
                            IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero
                          );
                if (hFile == INVALID_HANDLE_VALUE)
                    throw new MMException(Marshal.GetLastWin32Error());

                map._fileName = fileName;
            }

            // Map the file.
            map._hMap = Win32MapApis.CreateFileMapping(
                        hFile, IntPtr.Zero, (int)protection,
                        (int)((maxSize >> 32) & 0xFFFFFFFF),
                        (int)(maxSize & 0xFFFFFFFF), name
                    );
           
            if (hFile != INVALID_HANDLE_VALUE) Win32MapApis.CloseHandle(hFile);
            if (map._hMap == NULL_HANDLE)
                throw new MMException(Marshal.GetLastWin32Error());

            map._protection = protection;
            map._maxSize = maxSize;

            return map;
        }

        #endregion 
        
        public static MemoryMappedFile Open(FileMapAccessType access, String name)
        {
            MemoryMappedFile map = new MemoryMappedFile
            {
                _hMap = Win32MapApis.OpenFileMapping((int)access, false, name)
            };

            if (map._hMap == NULL_HANDLE)
                throw new MMException(Marshal.GetLastWin32Error());
            map._maxSize = -1; 
            return map;
        }
       
        public void Close()
        {
            Dispose(true);
        }

        public IntPtr MapView(FileMapAccessType access, ref long offset, ref long mapSize)
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Memory file already closed");

            // Grab system info to get the allocation granularity.
            SYSTEM_INFO pSI = new SYSTEM_INFO();
            Win32MapApis.GetSystemInfo( ref pSI );
            
            // Make sure the offset is aligned.
            offset -= offset % pSI.dwAllocationGranularity;

            // Find the largest contiguous virtual address space.
            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();
            long address = 0;
            const uint MEM_FREE = 0x10000;
            long freestart = 0, largestFreestart = 0;
            long free = 0, largestFree = 0;
            bool recording = false;
            while ( true )
            {                
                int numBytes = Win32MapApis.VirtualQueryEx(System.Diagnostics.Process.GetCurrentProcess().Handle, (IntPtr)address, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (numBytes != Marshal.SizeOf(mbi))
                {
                    break;
                }

                if (mbi.State == MEM_FREE)
                {
                    if (!recording)
                        freestart = address;
                    free += mbi.RegionSize.ToInt64();
                    recording = true;
                }
                else
                {
                    if (recording)
                    {
                        if (free > largestFree)
                        {
                            largestFree = free;
                            largestFreestart = freestart;
                        }
                    }
                    free = 0;
                    recording = false;
                }

                address += mbi.RegionSize.ToInt64();
            }

            // Make sure the address is aligned
            if (largestFreestart % pSI.dwAllocationGranularity != 0)
            {
                long diff = pSI.dwAllocationGranularity - (largestFreestart % pSI.dwAllocationGranularity);
                largestFreestart += diff;
                largestFree -= diff;
            }

            // Cap map size
            if (mapSize > largestFree)
                mapSize = largestFree;

            // Edge case
            if (offset + mapSize > _fileSize)
            {
                long diff = offset + mapSize - _fileSize;
                if (diff % pSI.dwAllocationGranularity != 0)
                {
                    diff -= diff % pSI.dwAllocationGranularity;
                    diff += pSI.dwAllocationGranularity;
                }
                offset -= diff;
            }

            IntPtr baseAddress = Win32MapApis.MapViewOfFileEx(
                _hMap, (int)access,
                (int)((offset >> 32) & 0xFFFFFFFF),
                (int)(offset & 0xFFFFFFFF ),
                (IntPtr)mapSize, (IntPtr)largestFreestart);

            if (baseAddress == IntPtr.Zero)
                throw new MMException(Marshal.GetLastWin32Error());

            return baseAddress;
        }

        public MapViewStream MapAsStream()
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Memory file already closed");

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
