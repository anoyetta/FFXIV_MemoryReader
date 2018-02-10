using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    class MemoryReader : IDisposable
    {
        private Process _process;
        private IntPtr mobArrayAddress = IntPtr.Zero;

        Signature MobArray = new Signature()
        {
            Pattern = "488b420848c1e8033da701000077248bc0488d0d",
            PointerPath = new long[] { 0L },
            Architecture = Architecture.x64_RIP_relative,

        };


        public MemoryReader(Process process)
        {
            _process = process;

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private bool GetPointerAddress()
        {
            return true;
        }


        private List<IntPtr> SigScan(Signature signature)
        {

            if (signature.Pattern == null || signature.Pattern.Length % 2 != 0)
            {
                return new List<IntPtr>();
            }

            // 1byte = 2char
            byte?[] patternByteArray = new byte?[signature.Pattern.Length / 2];

            // Convert Pattern to Array of Byte
            for (int i = 0; i < signature.Pattern.Length / 2; i++)
            {
                string text = signature.Pattern.Substring(i * 2, 2);
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

            // スキャン開始位置が最後まで行ったら検索
            while (intPtr_Scannning.ToInt64() < intPtr_EndOfModuleMemory.ToInt64())
            {
                IntPtr nSize = new IntPtr(splitSizeOfMemory);

                // (最後のsplitで) 残ったメモリがsplitSizeより小さかった場合は、残りのサイズにする
                if (IntPtr.Add(intPtr_Scannning, splitSizeOfMemory).ToInt64() > intPtr_EndOfModuleMemory.ToInt64())
                {
                    nSize = (IntPtr)(intPtr_EndOfModuleMemory.ToInt64() - intPtr_Scannning.ToInt64());
                }

                IntPtr intPtr_NumberOfBytesRead = IntPtr.Zero;
                if (NativeMethods.ReadProcessMemory(_process.Handle, intPtr_Scannning, splitMemoryArray, nSize, ref intPtr_NumberOfBytesRead))
                {
                    int num2 = 0;
                    // 切り出したメモリ内を1バイトずつずらしてPatternと合っているか確認していく
                    while ((long)num2 < intPtr_NumberOfBytesRead.ToInt64() - (long)patternByteArray.Length - 4L + 1L)
                    {
                        int matchCount = 0;
                        for (int j = 0; j < patternByteArray.Length; j++)
                        {
                            // ??だった部分はマッチしたとしてスルー
                            if (!patternByteArray[j].HasValue)
                            {
                                matchCount++;
                            }
                            else
                            {
                                if (patternByteArray[j].Value != splitMemoryArray[num2 + j])
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

                            switch (signature.Architecture)
                            {
                                case Architecture.x64:
                                    item = new IntPtr(BitConverter.ToInt64(splitMemoryArray, num2 + patternByteArray.Length));
                                    item = new IntPtr(item.ToInt64());
                                    break;
                                case Architecture.x64_RIP_relative:
                                    item = new IntPtr(BitConverter.ToInt32(splitMemoryArray, num2 + patternByteArray.Length));
                                    item = new IntPtr(item.ToInt64() + intPtr_Scannning.ToInt64() + (long)num2 + (long)patternByteArray.Length + 4L);
                                    break;
                                case Architecture.x86:
                                    item = new IntPtr(BitConverter.ToInt32(splitMemoryArray, num2 + patternByteArray.Length));
                                    item = new IntPtr(item.ToInt64());
                                    break;
                            }

                            // ToDo: 重複排除して追加する！
                            if (item != IntPtr.Zero && !list.Contains(item))
                            {
                                list.Add(item);
                            }
                        }
                        num2++;
                    }
                }
                intPtr_Scannning = IntPtr.Add(intPtr_Scannning, splitSizeOfMemory - patternByteArray.Length);
            }


            // この時点で list には Pattern の直後のアドレスが入っってる
            return list;

        }
    }
}
