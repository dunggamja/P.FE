using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Battle
{
    public enum EnumBuffTarget
    {
        None,

        Owner,
        Target,
    }

    public enum EnumBuffStatus
    {
        None,

        Unit_HP               = 1,   // 유닛, 체력
        Unit_Strength         = 2,   // 유닛, 힘
        Unit_Magic            = 3,   // 유닛, 마력
        Unit_Skill            = 4,   // 유닛, 기술
        Unit_Speed            = 5,   // 유닛, 속도
        Unit_Luck             = 6,   // 유닛, 행운
        Unit_Defense          = 7,   // 유닛, 수비
        Unit_Resistance       = 8,   // 유닛, 마방
        Unit_Movement         = 9,   // 유닛, 이동력
        Unit_Weight           = 10,  // 유닛, 중량(체격)
                              
        Weapon_Might          = 1001, // 무기, 위력
        Weapon_Hit            = 1002, // 무기, 명중
        Weapon_Critical       = 1003, // 무기, 필살
        Weapon_Weight         = 1004, // 무기, 무게
        Weapon_Dodge          = 1005, // 무기, 회피
        Weapon_Dodge_Critical = 1006, // 무기, 필살 회피
        Weapon_Range          = 1007, // 무기, 사정
                              
                              
        System_Damage_Physic  = 2001, // 시스템, 물리 데미지
        System_Damage_Magic   = 2002, // 시스템, 마법 데미지
        //Battle_Speed        = 2003, // 시스템, 속도  
        //Battle_Defense      = 2004, // 시스템, 수비
        //Battle_Resistance   = 2005, // 시스템, 마방
        System_Critical_Rate  = 2006, // 시스템, 필살 확률
        System_Dodge_Rate     = 2007, // 시스템, 회피 확률
        System_TurnSequence   = 2008, // 시스템, 행동 순서 
        System_TurnCount      = 2009, // 시스템, 행동 횟수 
        System_AttackCount    = 2010, // 시스템, 공격 횟수 (행동당)
    }


    public struct BuffTarget : IEqualityComparer<BuffTarget>
    {
        //public int Type;
        //public int Situation;
        public int Target;
        public int Status;

        public bool Equals(BuffTarget x, BuffTarget y)
        {
            return
            //x.Type      == y.Type      &&
            //x.Situation == y.Situation &&
            x.Target    == y.Target    &&
            x.Status    == y.Status;
        }

        public int GetHashCode(BuffTarget obj)
        {
            return /*obj.Type ^*/ /*obj.Situation ^*/ obj.Target ^ obj.Status;
        }

        public readonly static BuffTarget  Empty = new BuffTarget
        {
            //Type      = 0,
            //Situation = 0,
            Target    = 0,
            Status    = 0
        };
    }


    public struct BuffValue
    {
        public float Multiply;
        public int   Add;

        public readonly static BuffValue Empty = new BuffValue { Multiply = 0f, Add = 0 };

        public static BuffValue operator +(BuffValue a)              => a;
        public static BuffValue operator -(BuffValue a)              => new BuffValue { Multiply = -a.Multiply, Add = -a.Add };
        public static BuffValue operator +(BuffValue a, BuffValue b) => new BuffValue { Multiply = a.Multiply + b.Multiply, Add = a.Add + b.Add };
        public static BuffValue operator -(BuffValue a, BuffValue b) => new BuffValue { Multiply = a.Multiply - b.Multiply, Add = a.Add - b.Add };

        public int Calculate(int _value)
        {
            return (int)(_value * (Multiply + 1f)) + Add;
        }
    }

    public struct Buff
    {
        public long             ID;
        public BuffTarget       Target;
        public BuffValue        Value;
        public List<ICondition> Conditions;
               

        public readonly static Buff Empty = new Buff
        {
            ID         = 0,
            Target     = BuffTarget.Empty,
            Value      = BuffValue.Empty,
            Conditions = null
        };

        public bool IsValidCondition(BattleObject _owner)
        {
            if (Conditions != null)
            {
                foreach (var e in Conditions)
                {
                    if (e != null && !e.IsValid(_owner))
                        return false;
                }
            }

            return true;
        }
    }

    public class BuffMananger
    {
        Dictionary<long, Buff>                m_list_buff              = new Dictionary<long, Buff>();
        Dictionary<BuffTarget, HashSet<long>> m_list_buff_id_by_target = new Dictionary<BuffTarget, HashSet<long>>();


        public Buff      GetBuff(long _id) => m_list_buff.TryGetValue(_id, out var node) ? node : Buff.Empty;

        public BuffValue Accumulate_BuffValue(BattleObject _owner, BuffTarget _target)
        {
            var result = BuffValue.Empty;

            if (m_list_buff_id_by_target.TryGetValue(_target, out var list_buff_id))
            {
                if (list_buff_id != null)
                {
                    foreach (var id in list_buff_id)
                    {
                        var buff = GetBuff(id);
                        if (buff.IsValidCondition(_owner))
                        {
                            result += buff.Value;
                        }
                    }
                }
            }

            return result;
        }
    }

}

