using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleStatus
{
    public enum EnumBuffGroup
    {
        None,

        Status,    // Status 연산 시
        System,    // 
    }

    public enum EnumBuffSituation
    {
        None,       // 상황에 관계없이 항시 적용

        OnAttack,   // 공격시 적용
        OnDefense,  // 방어시 적용
    }

    public enum EnumBuffTarget
    {
        None,

        Owner,
        //Target,
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

        Weapon_Might          = 101, // 무기, 위력
        Weapon_Hit            = 102, // 무기, 명중
        Weapon_Critical       = 103, // 무기, 필살
        Weapon_Weight         = 104, // 무기, 무게
        Weapon_Dodge          = 105, // 무기, 회피
        Weapon_Dodge_Critical = 106, // 무기, 필살 회피
        Weapon_Range          = 107, // 무기, 사정


        Battle_Damage_Physic  = 201, // 전투, 물리 데미지
        Battle_Damage_Magic   = 202, // 전투, 마법 데미지
        Battle_Speed          = 203, // 전투, 속도        
    }

    public struct BuffGroupParam : IEqualityComparer<BuffGroupParam>
    {
        public EnumBuffGroup     Group;
        public EnumBuffSituation Situation;
        public EnumBuffTarget    Target;
        public EnumBuffStatus    Status;

        public bool Equals(BuffGroupParam x, BuffGroupParam y)
        {
            return
            x.Group     == y.Group     &&
            x.Situation == y.Situation &&
            x.Target    == y.Target    &&
            x.Status    == y.Status;
        }

        public int GetHashCode(BuffGroupParam obj)
        {
            return 
                (int)obj.Group     ^
                (int)obj.Situation ^
                (int)obj.Target    ^
                (int)obj.Status;
        }

        public readonly static BuffGroupParam  Empty = new BuffGroupParam
        {
            Group     = EnumBuffGroup.None,
            Situation = EnumBuffSituation.None,
            Target    = EnumBuffTarget.None,
            Status    = EnumBuffStatus.None
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

        public int Calculate(int _value) => (int)(_value * (Multiply + 1f)) + Add;
    }

    public struct BuffNode
    {
        public long              ID;
        public BuffGroupParam    Group;
        public BuffValue         Value;

        

        public readonly static BuffNode Empty = new BuffNode
        {
            ID    = 0,
            Group = BuffGroupParam.Empty,
            Value = BuffValue.Empty
        };

        
    }

    public class BuffNodeManager
    {
        Dictionary<long, BuffNode>                m_list_buff             = new Dictionary<long, BuffNode>();
        Dictionary<BuffGroupParam, HashSet<long>> m_list_buff_id_by_group = new Dictionary<BuffGroupParam, HashSet<long>>();

        Dictionary<long, Stack<BuffNode>>         m_stack_buff_on_plan    = new Dictionary<long, Stack<BuffNode>>();


        public BuffNode  GetBuffNode(long _id) => m_list_buff.TryGetValue(_id, out var node) ? node : BuffNode.Empty;

        public BuffValue GetBuffValue(BuffGroupParam _group, bool _is_plan)
        {
            var buff_value = BuffValue.Empty;

            if (m_list_buff_id_by_group.TryGetValue(_group, out var list_buff_id))
            {
                if (list_buff_id != null)
                {
                    foreach (var id in list_buff_id)
                        buff_value += GetBuffNode(id).Value;
                }
            }

            return buff_value;
        }
    }

}

