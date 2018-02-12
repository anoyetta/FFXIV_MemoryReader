using System;
using System.Runtime.InteropServices;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);

    }
}
