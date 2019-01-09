using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    partial class Memory : IDisposable
    {
        NLog.Logger Logger;
        internal Process Process { get; private set; }

        private bool IsMemoryScanningComplete = false;

        internal enum PointerType
        {
            Target,
            MobArray,
            CameraInfo,
            Hotbar,
            Recast,
        }

        Dictionary<PointerType, Pointer> Pointers = new Dictionary<PointerType, Pointer>
        {
            {
                PointerType.Target,
                new Pointer
                {
                    Signature = "41bc000000e041bd01000000493bc47555488d0d",
                    PointerPath = new long[1] { 192 },
                    Architecture = Architecture.x64_RIP_relative,
                }
            },
            {
                PointerType.MobArray,
                new Pointer
                {
                    Signature = "488b420848c1e8033da701000077248bc0488d0d",
                    PointerPath = new long[0],
                    Architecture = Architecture.x64_RIP_relative,
                }
            },
            {
                PointerType.CameraInfo,
                new Pointer
                {
                    Signature = "10000000788301000000000000000000????????00000000FFFFFFFF00000000",
                    PointerPath = new long[0],
                    Architecture = Architecture.x64,
                }
            },
            {
                PointerType.Hotbar,
                new Pointer
                {
                    Signature = "9C640000000000000000000000000000F783843E2C2C0000000000000000000075692F69636F6E2F????????????????????????????????????????????????0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000????????????????0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                    PointerPath = new long[6] {56, 48, 40, 32, 0, 0},
                    Architecture = Architecture.x64,
                }
            },
            {
                PointerType.Recast,
                new Pointer
                {
                    Signature = "9C640000000000000000000000000000F783843E2C2C0000000000000000000075692F69636F6E2F????????????????????????????????????????????????0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000????????????????0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                    PointerPath = new long[5] {56, 24, 48, 32, 60},
                    Architecture = Architecture.x64,
                    RequirePeriodicScan = true,
                }
            }
        };

        internal Dictionary<PointerType, IntPtr> GetPointers()
        {
            return new Dictionary<PointerType, IntPtr>
            {
                { PointerType.Target, Pointers[PointerType.Target].Address },
                { PointerType.MobArray, Pointers[PointerType.MobArray].Address },
                { PointerType.CameraInfo, Pointers[PointerType.CameraInfo].Address },
                { PointerType.Hotbar, Pointers[PointerType.Hotbar].Address },
                { PointerType.Recast, Pointers[PointerType.Recast].Address },
            };
        }

        public Memory(Process process, NLog.Logger logger)
        {
            Logger = logger;
            Process = process;

            Logger.Info("MemoryReader Start.");

            ResolvePointers();
            IsMemoryScanningComplete = true;
        }

        public void Dispose()
        {
            Logger.Info("MemoryReader Dispose Called.");
            Pointers = new Dictionary<PointerType, Pointer>();
            Process = null;
            Logger.Info("MemoryReader Dispose Finished.");
        }

        public bool IsValid => ValidateProcess();
        public bool HasAllPointers => ValidatePointers();
        private bool ValidateProcess()
        {
            if (Process == null)
            {
                return false;
            }

            if (Process.HasExited)
            {
                return false;
            }

            return true;
        }

        private bool ValidatePointers()
        {
            // If Scanning is not completed, return true
            if (!IsMemoryScanningComplete)
            {
                return true;
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


        internal bool ResolvePointers(bool force = false, bool periodicScan = false, bool verbose = false)
        {
            bool success = true;

            foreach (var p in Pointers)
            {
                if (!force)
                {
                    if (periodicScan)
                    {
                        if(!p.Value.RequirePeriodicScan || p.Value.Address == IntPtr.Zero)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (p.Value.Address != IntPtr.Zero)
                        {
                            continue;
                        }
                    }
                }

                if (verbose)
                {
                    Logger.Info("ResolvePointer: {0, 10}: Start.", p.Key);
                }
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var currentAddress = p.Value.Address;
                var baseAddress = SearchLocations(p.Value.Signature, p.Value.Architecture).FirstOrDefault();
                if (baseAddress == IntPtr.Zero)
                {
                    success = false;
                }
                else
                {

                    var nextAddress = baseAddress;

                    foreach (var offset in p.Value.PointerPath)
                    {
                        baseAddress = new IntPtr(nextAddress.ToInt64() + offset);

                        switch (p.Value.Architecture)
                        {
                            case Architecture.x64:
                            case Architecture.x64_RIP_relative:
                                // RIP Relative なのは最初だけ？
                                var b_x64 = new byte[8];
                                Peek(baseAddress, b_x64);
                                nextAddress = new IntPtr(BitConverter.ToInt64(b_x64, 0));
                                break;
                            case Architecture.x86:
                                var b_x86 = new byte[4];
                                Peek(baseAddress, b_x86);
                                nextAddress = new IntPtr(BitConverter.ToInt32(b_x86, 0));
                                break;
                        }
                    }
                }
                stopWatch.Stop();

                if (currentAddress.ToInt64() != baseAddress.ToInt64())
                {
                    p.Value.Address = baseAddress;
                    if (p.Value.Address != IntPtr.Zero)
                    {
                        if (currentAddress == IntPtr.Zero)
                        {
                            Logger.Info("ResolvePointer: {0, 10}: Found.   ({1, 3}ms) Address is {2}", p.Key, stopWatch.ElapsedMilliseconds, p.Value.Address.ToInt64());

                        }
                        else
                        {
                            Logger.Info("ResolvePointer: {0, 10}: Changed. ({1, 3}ms) Address is {2}", p.Key, stopWatch.ElapsedMilliseconds, p.Value.Address.ToInt64());
                        }
                    }
                    if (verbose)
                    {
                        Logger.Info("ResolvePointer: {0,10}: Finish.  ({1, 3}ms)", p.Key, stopWatch.ElapsedMilliseconds);
                    }
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

            int moduleMemorySize = Process.MainModule.ModuleMemorySize;
            IntPtr baseAddress = Process.MainModule.BaseAddress;
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
                if (NativeMethods.ReadProcessMemory(Process.Handle, intPtr_Scannning, splitMemoryArray, nSize, ref intPtr_NumberOfBytesRead))
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

                        // if all bytes are match, it means "the pattern found"
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

                            // add the item if not contains already
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
            return NativeMethods.ReadProcessMemory(Process.Handle, address, buffer, nSize, ref zero);
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
