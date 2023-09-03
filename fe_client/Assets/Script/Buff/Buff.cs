using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EnumBuffTarget
{
    None,

    Owner,  // 소유자에게 버프 적용
    Target, // 타겟에게 버프 적용
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
                          
                          
    System_Damage         = 2001, // 시스템, 데미지
    //System_Damage_Magic   = 2002, // 시스템, 마법 데미지
    System_Critical       = 2006, // 시스템, 필살 확률
    //System_Dodge_Critical = 2007, // 시스템, 필살 회피 확률
    System_Hit            = 2008, // 시스템, 명중 확률
    //System_Dodge          = 2009, // 시스템, 회피 확률


    System_TurnSequence   = 2100, // 시스템, 행동 순서 
    System_TurnCount      = 2101, // 시스템, 행동 횟수 
    System_AttackCount    = 2102, // 시스템, 공격 횟수 (행동당)
}


public enum EnumBuffOption
{
    LimitOfUse,  // 횟수 제한
    LimitOfTurn, // 턴 제한
    Immutable,   // 변경 불가능

    COUNT
}


public struct BuffTarget : IEqualityComparer<BuffTarget>
{
    //public int Type;
    //public int Situation;
    public int Situation;
    public int Target;
    public int Status;

    public bool Equals(BuffTarget x, BuffTarget y)
    {
        return
        //x.Type      == y.Type      &&
        x.Situation == y.Situation &&
        x.Target    == y.Target    &&
        x.Status    == y.Status;
    }

    public int GetHashCode(BuffTarget obj)
    {
        return obj.Situation ^ obj.Target ^ obj.Status;
    }

    public readonly static BuffTarget  Empty = new BuffTarget
    {
        //Type      = 0,
        Situation = 0,
        Target    = 0,
        Status    = 0
    };

    public static BuffTarget Create(EnumSituationType _situation, EnumBuffTarget _target, EnumBuffStatus _status)
    {
        return new BuffTarget { Situation = (int)_situation, Target = (int)EnumBuffTarget.Owner, Status = (int)_status };
    }
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
        // multiplier 값이 0이하로 가는 경우는 없다.
        var multiplier = Math.Max(0f, Multiply + 1f);

        return (int)(_value * multiplier) + Add;
    }
}


public struct BuffOption
{
    // 
    int[] Values;


    public readonly static BuffOption Empty = new BuffOption
    {
        // -1은 사용하지 않는 옵션이라는 뜻.
        Values = new int[(int)EnumBuffOption.COUNT] { -1, -1, -1 }
    };


    public bool SetValue(EnumBuffOption _option, int _value)
    {
        if (Values == null)
            return false;

        var index = (int)_option;
        if (index < 0 || Values.Length <= index)
            return false;

        // 초기화 말고는 음수 셋팅을 막아두자.
        if (_value < 0)
            return false;

        Values[index] = _value;
        return true;
    }

    public int GetValue(EnumBuffOption _option)
    {
        if (Values == null)
            return 0;

        var index = (int)_option;
        if (index < 0 || Values.Length <= index)
            return 0;

        return Values[index];
    }

    public bool HasValue(EnumBuffOption _option) => 0 < GetValue(_option);
    public bool HasToVerify(EnumBuffOption _option) => -1 < GetValue(_option);

    public void IncreaseValue(EnumBuffOption _option) => SetValue(_option, GetValue(_option) + 1);
    public void DecreaseValue(EnumBuffOption _option) => SetValue(_option, GetValue(_option) - 1);


    public bool IsExpired()
    {
        // 횟수제한 체크.
        if (HasToVerify(EnumBuffOption.LimitOfUse) && !HasValue(EnumBuffOption.LimitOfUse))
            return true;

        // 턴 제한 체크
        if (HasToVerify(EnumBuffOption.LimitOfTurn) && !HasValue(EnumBuffOption.LimitOfTurn))
            return true;

        return false;
    }

    
    public void Process_Use()
    {
        DecreaseValue(EnumBuffOption.LimitOfUse);
    }

    public void Process_Turn()
    {
        DecreaseValue(EnumBuffOption.LimitOfTurn);
    }
}


public struct Buff
{
    public long             ID;
    public BuffTarget       Target;
    public BuffValue        Value;
    public BuffOption       Option;
    public List<ICondition> Conditions;

           

    public readonly static Buff Empty = new Buff
    {
        ID         = 0,
        Target     = BuffTarget.Empty,
        Value      = BuffValue.Empty,
        Option     = BuffOption.Empty,
        Conditions = null
    };

    public bool IsValidCondition(IOwner _owner)
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

    public bool IsExpired()
    {
        return Option.IsExpired();
    }
}

public class BuffMananger : IBuff
{
    Dictionary<long, Buff>                m_list_buff              = new Dictionary<long, Buff>();
    Dictionary<BuffTarget, HashSet<long>> m_list_buff_id_by_target = new Dictionary<BuffTarget, HashSet<long>>();


    public bool      AddBuff(Buff _buff)
    {
        if (_buff.ID == 0)
            return false;

        // 겹칠 경우 실패한다고 해두자.
        if (m_list_buff.ContainsKey(_buff.ID))
            return false;


        // ID에 등록
        m_list_buff.Add(_buff.ID, _buff);

        // 타겟정보에도 등록
        if (!m_list_buff_id_by_target.TryGetValue(_buff.Target, out var list_buff_id))
        {
            list_buff_id = new HashSet<long>();
            m_list_buff_id_by_target.Add(_buff.Target, list_buff_id);
        }
        list_buff_id.Add(_buff.ID);


        return true;
    }

    public Buff      GetBuff(long _id) => m_list_buff.TryGetValue(_id, out var node) ? node : Buff.Empty;

    public BuffValue Collect_BuffValue(IOwner _owner, BuffTarget _target)
    {
        var result = BuffValue.Empty;

        if (m_list_buff_id_by_target.TryGetValue(_target, out var list_buff_id))
        {
            if (list_buff_id != null)
            {
                foreach (var id in list_buff_id)
                {
                    var buff = GetBuff(id);
                    if (buff.IsExpired())
                        continue;

                    if (buff.IsValidCondition(_owner))
                    {
                        result += buff.Value;
                    }
                }
            }
        }

        return result;
    }


    


    #region IBuff Interface
    public BuffValue Collect(EnumSituationType _situation, IOwner _owner, EnumBuffStatus _status) 
    {
        return Collect_BuffValue(_owner, BuffTarget.Create(_situation, EnumBuffTarget.Owner, _status));
    }
    #endregion IBuff Interface
}


