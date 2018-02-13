using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    partial class Memory : IDisposable
    {
        NLog.Logger Logger;

        private Process _process;
        internal Process Process => _process;

        enum PointerType
        {
            MobArray,
        }

        Dictionary<PointerType, Pointer> Pointers = new Dictionary<PointerType, Pointer>
        {
            {
                PointerType.MobArray,
                new Pointer
                {
                    Signature = "488b420848c1e8033da701000077248bc0488d0d",
                    PointerPath = new long[0],
                    Architecture = Architecture.x64_RIP_relative,
                }
            }
        };


        public Memory(Process process, NLog.Logger logger)
        {
            Logger = logger;
            _process = process;

            Logger.Trace("MemoryReader Start.");
            ResolvePointers();
            foreach (var p in Pointers)
            {
                Logger.Trace("{0} -> {1}", p.Key, p.Value.Address.ToInt64());
            }
        }

        public void Dispose()
        {
            Logger.Trace("MemoryReader Dispose Called.");
        }

        public bool IsValid => ValidateProcess();
        private bool ValidateProcess()
        {
            if (_process == null)
            {
                return false;
            }

            if (_process.HasExited)
            {
                return false;
            }

            foreach (var p in Pointers)
            {
                if (p.Value.Address == IntPtr.Zero)
                {
                    return false;
                }
            }
            return true;
        }



        private bool ResolvePointers()
        {
            bool success = true;

            foreach (var p in Pointers)
            {
                var baseAddress = SearchLocations(p.Value.Signature, p.Value.Architecture).FirstOrDefault();
                if (baseAddress == IntPtr.Zero)
                {
                    success = false;
                    p.Value.Address = baseAddress;
                }
                else
                {

                    var nextAddress = baseAddress;

                    foreach (var offset in p.Value.PointerPath)
                    {
                        baseAddress = new IntPtr(nextAddress.ToInt64() + offset);

                        //IntPtr lpNumberOfBytesRead = IntPtr.Zero;

                        switch (p.Value.Architecture)
                        {
                            case Architecture.x64:
                            case Architecture.x64_RIP_relative:
                                // RIP Relative なのは最初だけ？
                                var b_x64 = new byte[8];
                                Peek(baseAddress, b_x64);
                                //NativeMethods.ReadProcessMemory(_process.Handle, baseAddress, b_x64, new IntPtr(b_x64.Length), ref lpNumberOfBytesRead);
                                nextAddress = new IntPtr(BitConverter.ToInt64(b_x64, 0));
                                break;
                            case Architecture.x86:
                                var b_x86 = new byte[4];
                                Peek(baseAddress, b_x86);
                                //NativeMethods.ReadProcessMemory(_process.Handle, baseAddress, b_x86, new IntPtr(b_x86.Length), ref lpNumberOfBytesRead);
                                nextAddress = new IntPtr(BitConverter.ToInt32(b_x86, 0));
                                break;
                        }
                    }
                    p.Value.Address = baseAddress;
                }
            }

            return success;
        }


        private List<IntPtr> SearchLocations(string pattern, Architecture architecture)
        {

            if (pattern == null || pattern.Length % 2 != 0)
            {
                return new List<IntPtr>();
            }

            if (pattern.Length == 0)
            {
                return new List<IntPtr>() { IntPtr.Zero };
            }

            // 1byte = 2char
            byte?[] patternByteArray = new byte?[pattern.Length / 2];

            // Convert Pattern to "Array of Byte"
            for (int i = 0; i < pattern.Length / 2; i++)
            {
                string text = pattern.Substring(i * 2, 2);
                if (text == "??")
                {
                    patternByteArray[i] = null;
                }
                else
                {
                    patternByteArray[i] = new byte?(Convert.ToByte(text, 16));
                }
            }

            int moduleMemorySize = _process.MainModule.ModuleMemorySize;
            IntPtr baseAddress = _process.MainModule.BaseAddress;
            IntPtr intPtr_EndOfModuleMemory = IntPtr.Add(baseAddress, moduleMemorySize);
            IntPtr intPtr_Scannning = baseAddress;

            int splitSizeOfMemory = 65536;
            byte[] splitMemoryArray = new byte[splitSizeOfMemory];

            List<IntPtr> list = new List<IntPtr>();

            // while loop for scan all memory
            while (intPtr_Scannning.ToInt64() < intPtr_EndOfModuleMemory.ToInt64())
            {
                IntPtr nSize = new IntPtr(splitSizeOfMemory);

                // if remaining memory size is less than splitSize, change nSize to remaining size
                if (IntPtr.Add(intPtr_Scannning, splitSizeOfMemory).ToInt64() > intPtr_EndOfModuleMemory.ToInt64())
                {
                    nSize = (IntPtr)(intPtr_EndOfModuleMemory.ToInt64() - intPtr_Scannning.ToInt64());
                }

                IntPtr intPtr_NumberOfBytesRead = IntPtr.Zero;

                // read memory
                if (NativeMethods.ReadProcessMemory(_process.Handle, intPtr_Scannning, splitMemoryArray, nSize, ref intPtr_NumberOfBytesRead))
                {
                    int num = 0;

                    // slide start point byte bu byte, check with patternByteArray
                    while ((long)num < intPtr_NumberOfBytesRead.ToInt64() - (long)patternByteArray.Length)
                    {
                        int matchCount = 0;
                        for (int j = 0; j < patternByteArray.Length; j++)
                        {
                            // pattern "??" have a null value. in this case, skip the check.
                            if (!patternByteArray[j].HasValue)
                            {
                                matchCount++;
                            }
                            else
                            {
                                if (patternByteArray[j].Value != splitMemoryArray[num + j])
                                {
                                    break;
                                }
                                matchCount++;
                            }
                        }

                        // 最後まで行ったら見つかったと言うこと
                        if (matchCount == patternByteArray.Length)
                        {
                            IntPtr item = IntPtr.Zero;
                            switch (architecture)
                            {
                                case Architecture.x64:
                                    item = new IntPtr(BitConverter.ToInt64(splitMemoryArray, num + patternByteArray.Length));
                                    item = new IntPtr(item.ToInt64());
                                    break;
                                case Architecture.x64_RIP_relative:
                                    item = new IntPtr(BitConverter.ToInt32(splitMemoryArray, num + patternByteArray.Length));
                                    item = new IntPtr(item.ToInt64() + intPtr_Scannning.ToInt64() + (long)num + (long)patternByteArray.Length + 4L);
                                    break;
                                case Architecture.x86:
                                    item = new IntPtr(BitConverter.ToInt32(splitMemoryArray, num + patternByteArray.Length));
                                    item = new IntPtr(item.ToInt64());
                                    break;
                            }

                            // ToDo: 重複排除して追加する！
                            if (item != IntPtr.Zero && !list.Contains(item))
                            {
                                list.Add(item);
                            }

                        }
                        num++;
                    }
                }
                intPtr_Scannning = IntPtr.Add(intPtr_Scannning, splitSizeOfMemory - patternByteArray.Length);
            }


            // この時点で list には Pattern の直後のアドレスが入っってる
            return list;

        }

        private byte[] GetByteArray(IntPtr address, int length)
        {
            var data = new byte[length];
            Peek(address, data);
            return data;
        }

        private bool Peek(IntPtr address, byte[] buffer)
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr nSize = new IntPtr(buffer.Length);
            return NativeMethods.ReadProcessMemory(_process.Handle, address, buffer, nSize, ref zero);
        }

        private static string GetStringFromBytes(byte[] source, int offset = 0, int size = 256)
        {
            var bytes = new byte[size];
            Array.Copy(source, offset, bytes, 0, size);
            var realSize = 0;
            for (var i = 0; i < size; i++)
            {
                if (bytes[i] != 0)
                {
                    continue;
                }
                realSize = i;
                break;
            }
            Array.Resize(ref bytes, realSize);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }


    }
}
