using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    internal class Pointer
    {
        public string Signature { get; set; }
        public IntPtr Address { get; set; } = IntPtr.Zero;
        public long[] PointerPath { get; set; }
        public Architecture Architecture { get; set; }
    }

    enum Architecture
    {
        x64 = 0,
        x64_RIP_relative = 1,
        x86 = 2,
    }


}
