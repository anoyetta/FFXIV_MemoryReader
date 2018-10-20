using System;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    internal class Pointer
    {
        public string Signature { get; set; }
        public IntPtr Address { get; set; } = IntPtr.Zero;
        public long[] PointerPath { get; set; }
        public Architecture Architecture { get; set; }
        public bool RequirePeriodicScan { get; set; } = false;
    }

    enum Architecture
    {
        x64 = 0,
        x64_RIP_relative = 1,
        x86 = 2,
    }


}
