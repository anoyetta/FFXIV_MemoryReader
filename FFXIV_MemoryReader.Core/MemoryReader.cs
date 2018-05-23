using System;
using System.Collections.Generic;
using TamanegiMage.FFXIV_MemoryReader.Model;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    partial class Memory
    {
        private const int combatantDataSize = 16192; //0x3F40

        internal unsafe List<Model.CombatantV1> GetCombatantsV1()
        {
            List<CombatantV1> result = new List<CombatantV1>();
            if (Pointers[PointerType.MobArray].Address == IntPtr.Zero)
            {
                return result;
            }

            try
            {
                int num = 344;
                int sz = 8;
                byte[] source = GetByteArray(Pointers[PointerType.MobArray].Address, sz * num);
                if (source == null || source.Length == 0) { return result; }

                for (int i = 0; i < num; i++)
                {
                    IntPtr p;
                    fixed (byte* bp = source) p = new IntPtr(*(Int64*)&bp[i * sz]);

                    if (!(p == IntPtr.Zero))
                    {
                        byte[] c = GetByteArray(p, combatantDataSize);
                        CombatantV1 combatant = GetCombatantFromByteArray(c);
                        if (combatant.type != ObjectType.PC && combatant.type != ObjectType.Monster)
                        {
                            continue;
                        }
                        if (combatant.ID != 0 && combatant.ID != 3758096384u && !result.Exists((CombatantV1 x) => x.ID == combatant.ID))
                        {
                            combatant.Order = i;
                            result.Add(combatant);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        public unsafe CombatantV1 GetCombatantFromByteArray(byte[] source)
        {
            int offset = 0;
            CombatantV1 combatant = new CombatantV1();
            fixed (byte* p = source)
            {
                combatant.Name = GetStringFromBytes(source, 48);
                combatant.ID = *(uint*)&p[116];
                combatant.OwnerID = *(uint*)&p[132];
                if (combatant.OwnerID == 3758096384u)
                {
                    combatant.OwnerID = 0u;
                }
                combatant.type = (ObjectType)p[140];
                combatant.EffectiveDistance = p[146];

                offset = 160;
                combatant.PosX = *(Single*)&p[offset];
                combatant.PosZ = *(Single*)&p[offset + 4];
                combatant.PosY = *(Single*)&p[offset + 8];
                combatant.Heading = *(Single*)&p[offset + 16];

                combatant.TargetID = *(uint*)&p[5744];

                offset = 5880;
                if (combatant.type == ObjectType.PC || combatant.type == ObjectType.Monster)
                {
                    combatant.CurrentHP = *(int*)&p[offset + 8];
                    combatant.MaxHP = *(int*)&p[offset + 12];
                    combatant.CurrentMP = *(int*)&p[offset + 16];
                    combatant.MaxMP = *(int*)&p[offset + 20];
                    combatant.CurrentTP = *(short*)&p[offset + 24];
                    combatant.MaxTP = 1000;
                    combatant.Job = p[offset + 64];
                    combatant.Level = p[offset + 66];

                    // Status aka Buff,Debuff
                    combatant.Statuses = new List<StatusV1>();
                    const int StatusEffectOffset = 6072;
                    const int statusSize = 12;

                    int statusCountLimit = 60;
                    if (combatant.type == ObjectType.PC) statusCountLimit = 30;

                    var statusesSource = new byte[statusCountLimit * statusSize];
                    Buffer.BlockCopy(source, StatusEffectOffset, statusesSource, 0, statusCountLimit * statusSize);
                    for (var i = 0; i < statusCountLimit; i++)
                    {
                        var statusBytes = new byte[statusSize];
                        Buffer.BlockCopy(statusesSource, i * statusSize, statusBytes, 0, statusSize);
                        var status = new StatusV1
                        {
                            StatusID = BitConverter.ToInt16(statusBytes, 0),
                            Stacks = statusBytes[2],
                            Duration = BitConverter.ToSingle(statusBytes, 4),
                            CasterID = BitConverter.ToUInt32(statusBytes, 8),
                            IsOwner = false,
                        };

                        if (status.IsValid())
                        {
                            combatant.Statuses.Add(status);
                        }
                    }

                    // Cast
                    combatant.Casting = new CastV1
                    {
                        ID = *(short*)&p[6452],
                        TargetID = *(uint*)&p[6464],
                        Progress = *(Single*)&p[6500],
                        Time = *(Single*)&p[6504],
                    };
                }
                else
                {
                    combatant.CurrentHP =
                    combatant.MaxHP =
                    combatant.CurrentMP =
                    combatant.MaxMP =
                    combatant.MaxTP =
                    combatant.CurrentTP = 0;
                    combatant.Statuses = new List<StatusV1>();
                    combatant.Casting = new CastV1();
                }
            }
            return combatant;
        }

        public unsafe CameraInfoV1 GetCameraInfoV1()
        {
            CameraInfoV1 result = new CameraInfoV1();

            if (Pointers[PointerType.CameraInfo].Address == IntPtr.Zero)
            {
                return result;
            }

            try
            {
                byte[] source = GetByteArray(Pointers[PointerType.CameraInfo].Address, 512);
                if (source == null || source.Length == 0) { return result; }

                fixed (byte* p = source)
                {
                    int offset = 0x120;
                    result.Mode = p[offset];
                    result.Heading = *(Single*)&p[offset + 36];
                    result.Elevation = *(Single*)&p[offset + 40];
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        #region HotbarRecast

        public unsafe List<HotbarRecastV1> GetHotbarRecastV1()
        {
            List<HotbarRecastV1> result = new List<HotbarRecastV1>();
            if (Pointers[PointerType.Hotbar].Address == IntPtr.Zero || Pointers[PointerType.Recast].Address == IntPtr.Zero)
            {
                return result;
            }

            try
            {
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_1));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_2));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_3));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_4));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_5));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_6));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_7));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_8));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_9));
                result.AddRange(GetRecastListByHotbar(HotbarType.HOTBAR_10));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_1));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_2));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_3));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_4));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_5));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_6));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_7));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_HOTBAR_8));
                result.AddRange(GetRecastListByHotbar(HotbarType.PETBAR));
                result.AddRange(GetRecastListByHotbar(HotbarType.X_PETBAR));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            return result;

        }

        public unsafe List<HotbarRecastV1> GetRecastListByHotbar(HotbarType hotbarType)
        {
            List<HotbarRecastV1> result = new List<HotbarRecastV1>();
            if (Pointers[PointerType.Hotbar].Address == IntPtr.Zero ||
                Pointers[PointerType.Recast].Address == IntPtr.Zero)
            {
                return result;
            }


            try
            {

                // set hotbar slot count
                int nItems;
                bool canUseKeyBinds;
                switch (hotbarType)
                {
                    case HotbarType.X_HOTBAR_1:
                    case HotbarType.X_HOTBAR_2:
                    case HotbarType.X_HOTBAR_3:
                    case HotbarType.X_HOTBAR_4:
                    case HotbarType.X_HOTBAR_5:
                    case HotbarType.X_HOTBAR_6:
                    case HotbarType.X_HOTBAR_7:
                    case HotbarType.X_HOTBAR_8:
                    case HotbarType.X_PETBAR:
                        nItems = 16;
                        canUseKeyBinds = false;
                        break;
                    default:
                        nItems = 12;
                        canUseKeyBinds = true;
                        break;
                }

                // set address and area for hotbar
                var hotbarItemSize = 0xD8; // 216L
                var hotbarMemoryAreaSize = hotbarItemSize * 16;
                var hotbarAddress = IntPtr.Add(Pointers[PointerType.Hotbar].Address, hotbarMemoryAreaSize * (int)hotbarType);

                // set address and area for recast
                var recastItemSize = 0x28; // 40L
                var recastMemoryAreaSize = recastItemSize * 16;
                var recastAddress = IntPtr.Add(Pointers[PointerType.Recast].Address, recastMemoryAreaSize * (int)hotbarType);

                // get memory bytes
                var hotbarMemoryAreaBytes = GetByteArray(hotbarAddress, hotbarMemoryAreaSize);
                var recastMemoryAreaBytes = GetByteArray(recastAddress, recastMemoryAreaSize);

                for (var i = 0; i < nItems; i++)
                {
                    try
                    {
                        var hotbarItemBytes = new byte[hotbarItemSize];
                        var recastItemBytes = new byte[recastItemSize];

                        // copy an item from memory area bytes
                        Buffer.BlockCopy(hotbarMemoryAreaBytes, hotbarItemSize * i, hotbarItemBytes, 0, hotbarItemSize);
                        Buffer.BlockCopy(recastMemoryAreaBytes, recastItemSize * i, recastItemBytes, 0, recastItemSize);

                        // get hotbaritem values
                        var name = GetStringFromBytes(hotbarItemBytes, 0, 80);
                        var keyBinds = GetStringFromBytes(hotbarItemBytes, 103, 40);
                        var id = BitConverter.ToInt16(hotbarItemBytes, 150);

                        // if name is not set, this slot is empty.
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        // if keybinds set, remove keybinds string from name
                        if (canUseKeyBinds && !String.IsNullOrWhiteSpace(keyBinds))
                        {
                            name = name.Replace($" {keyBinds}", "");
                        }

                        // get recastitem values
                        var category = BitConverter.ToInt32(recastItemBytes, 0);
                        var type = BitConverter.ToInt32(recastItemBytes, 8);
                        var recastId = BitConverter.ToInt32(recastItemBytes, 12); // same as hotbar itemid? but not used.
                        var icon = BitConverter.ToInt32(recastItemBytes, 16);
                        var coolDownPercent = BitConverter.ToInt32(recastItemBytes, 20);
                        var isAvailable = BitConverter.ToBoolean(recastItemBytes, 24);
                        var remainingorcost = BitConverter.ToInt32(recastItemBytes, 28);
                        remainingorcost = remainingorcost < 0 ? 0 : remainingorcost; // if minus, set 0.
                        var amount = BitConverter.ToInt32(recastItemBytes, 32);
                        var inRange = BitConverter.ToBoolean(recastItemBytes, 36);
                        var isProcOrcombo = recastItemBytes[21] > 0;

                        /**
                         * remainingorcost means "Remaining OR Cost"
                         * lower left value of icon.
                         * if CoolingDown, this is Remaining Time, or not this is Cost.
                         * 
                         **/


                        // TYPE filter 
                        //   type:0  is system
                        //   type:1  is skill/magic
                        //   type:2  is item
                        //   type:<0 is error
                        if (type == 2 || type < 0)
                        {
                            continue;
                        }

                        result.Add(new HotbarRecastV1
                        {
                            HotbarType = hotbarType,
                            Slot = i,
                            ID = id,
                            Name = name,

                            Category = category,
                            Type = type,
                            Icon = icon,
                            CoolDownPercent = coolDownPercent,
                            IsAvailable = isAvailable,
                            RemainingOrCost = remainingorcost,
                            Amount = amount,
                            InRange = inRange,
                            IsProcOrCombo = isProcOrcombo,
                        });

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        #endregion 

    }
}
