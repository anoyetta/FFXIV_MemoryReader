using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace TamanegiMage.FFXIV_MemoryReader.Helper
{
    static class ProcessHelper
    {
        public static IList<Process> GetFFXIVProcessList()
            => (from x in Process.GetProcessesByName("ffxiv_dx11")
                where !x.HasExited && x.MainModule != null && x.MainModule.ModuleName == "ffxiv_dx11.exe"
                select x).ToList<Process>();


        public static Process GetFFXIVProcess(int pid = 0)
        {
            Process result = null;

            try
            {
                IList<Process> list = GetFFXIVProcessList();
                if (pid == 0)
                {
                    if (list.Any<Process>())
                    {
                        result = (from x in list orderby x.Id select x).FirstOrDefault<Process>();
                    }
                    else
                    {
                        result = null;
                    }
                }
                else
                {
                    result = list.FirstOrDefault((Process x) => x.Id == pid);
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }
        
    }
}
