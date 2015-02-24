using System;
using System.Runtime.InteropServices;

namespace TTG.NavalWar.NWData.Util.MemoryMapping
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SYSTEM_INFO
    {
        internal _PROCESSOR_INFO_UNION uProcessorInfo;
        public uint dwPageSize;
        public UIntPtr lpMinimumApplicationAddress;
        public UIntPtr lpMaximumApplicationAddress;
        public IntPtr dwActiveProcessorMask;
        public uint dwNumberOfProcessors;
        public uint dwProcessorType;
        public uint dwAllocationGranularity;
        public ushort dwProcessorLevel;
        public ushort dwProcessorRevision;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
    public struct _PROCESSOR_INFO_UNION
    {
        [FieldOffset( 0 )]
        internal uint dwOemId;
        [FieldOffset( 0 )]
        internal ushort wProcessorArchitecture;
        [FieldOffset( 2 )]
        internal ushort wReserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MEMORY_BASIC_INFORMATION
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public uint AllocationProtect;
        public IntPtr RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }

    public enum FileMapAccessType : uint
    {
        Copy = 0x01,
        Write = 0x02,
        Read = 0x04,
        AllAccess = 0x08,
        Execute = 0x20,
    }

    internal class Win32MapApis
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
           String lpFileName, int dwDesiredAccess, int dwShareMode,
           IntPtr lpSecurityAttributes, int dwCreationDisposition,
           int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(
           IntPtr hFile, IntPtr lpAttributes, int flProtect,
           int dwMaximumSizeLow, int dwMaximumSizeHigh,
           String lpName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool FlushViewOfFile(
           IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(
           IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh,
           int dwFileOffsetLow, IntPtr dwNumBytesToMap);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFileEx( IntPtr hFileMappingObject,
           int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow,
           IntPtr dwNumberOfBytesToMap, IntPtr lpBaseAddress );

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(
           int dwDesiredAccess, bool bInheritHandle, String lpName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr VirtualQuery(ref IntPtr lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, IntPtr dwLength);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int VirtualQueryEx( IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength );
    }
}
