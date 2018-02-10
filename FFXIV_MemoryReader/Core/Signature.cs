using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    
    class Signature
    {
        internal string Pattern { get; set; }
        internal long[] PointerPath { get; set; }
        internal Architecture Architecture { get; set; }

    }

    enum Architecture
    {
        x64 = 0,
        x64_RIP_relative = 1,
        x86 = 2,
    }

}
