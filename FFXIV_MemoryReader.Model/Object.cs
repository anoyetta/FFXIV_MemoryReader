using System;
using System.Collections.Generic;

namespace TamanegiMage.FFXIV_MemoryReader.Model
{
    public enum ObjectType : byte
    {
        Unknown = 0x00,
        PC = 0x01,
        Monster = 0x02,
        NPC = 0x03,
        Aetheryte = 0x05,
        Gathering = 0x06,
        Minion = 0x09
    }
    public enum JobEnum : byte
    {
        UNKNOWN,
        GLD, // 1
        PGL, // 2
        MRD, // 3
        LNC, // 4
        ARC, // 5
        CNJ, // 6
        THM, // 7
        CRP, // 8
        BSM, // 9
        ARM, // 10
        GSM, // 11
        LTW, // 12
        WVR, // 13
        ALC, // 14
        CUL, // 15
        MIN, // 15
        BTN, // 17
        FSH, // 18
        PLD, // 19
        MNK, // 20
        WAR, // 21
        DRG, // 22
        BRD, // 23
        WHM, // 24
        BLM, // 25
        ACN, // 26
        SMN, // 27
        SCH, // 28
        ROG, // 29
        NIN, // 30
        MCH, // 31
        DRK, // 32
        AST, // 33
        SAM, // 34
        RDM  // 35
    }

    public class CombatantV1
    {
        public uint ID;
        public uint OwnerID;
        public int Order;
        public ObjectType type;
        public uint TargetID;

        public byte Job;
        public byte Level;
        public string Name;

        public int CurrentHP;
        public int MaxHP;
        public int CurrentMP;
        public int MaxMP;
        public short MaxTP;
        public short CurrentTP;

        public List<StatusV1> Statuses;
        public CastV1 Casting;

        public Single PosX;
        public Single PosY;
        public Single PosZ;
        public Single Heading;
        public byte EffectiveDistance;
        public string Distance;
        public string HorizontalDistance;


        public float GetDistanceTo(CombatantV1 target)
        {
            var distanceX = (float)Math.Abs(PosX - target.PosX);
            var distanceY = (float)Math.Abs(PosY - target.PosY);
            var distanceZ = (float)Math.Abs(PosZ - target.PosZ);
            return (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY) + (distanceZ * distanceZ));
        }

        public float GetHorizontalDistanceTo(CombatantV1 target)
        {
            var distanceX = (float)Math.Abs(PosX - target.PosX);
            var distanceY = (float)Math.Abs(PosY - target.PosY);
            return (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
        }

    }

    public class StatusV1
    {
        //public Combatant SourceCombatant;
        public short StatusID;
        public string StatusName;
        public byte Stacks;
        public float Duration;
        public uint CasterID;
        public bool IsOwner;
        public bool IsValid()
        {
            return StatusID > 0 && Duration <= 86400 && CasterID > 0;
        }
    }
    public class CastV1
    {
        public short ID;
        public uint TargetID;
        public float Progress;
        public float Time;
        public bool IsValid()
        {
            return ID > 0 && TargetID > 0;
        }
    }
}
