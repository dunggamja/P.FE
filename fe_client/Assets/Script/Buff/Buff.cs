using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;

public enum EnumBuffContentsType
{
    None,
    Item_Equipment  = 1,  // 장착 시 적용.
    Item_Consumable = 2, // 사용 시 적용,
    Item_Accessory  = 3,  // 소지만 하고 있어도 적용.
}

public enum EnumBuffTarget
{
    None,

    Owner  = 1,  // 소유자에게 버프 적용
    Target = 2, // 타겟에게 버프 적용
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
    // Unit_Weight           = 10,  // 유닛, 중량(체격)
                          
    Weapon_Might          = 1001, // 무기, 위력
    Weapon_Hit            = 1002, // 무기, 명중
    Weapon_Critical       = 1003, // 무기, 필살
    Weapon_Weight         = 1004, // 무기, 무게
    Weapon_Dodge          = 1005, // 무기, 회피
    Weapon_Dodge_Critical = 1006, // 무기, 필살 회피
    Weapon_Range          = 1007, // 무기, 사거리
    Weapon_Range_Min      = 1008, // 무기, 사거리 (최소)
                          
                          
    System_Damage         = 2001, // 시스템, 데미지
    System_Critical       = 2006, // 시스템, 필살 확률
    System_Hit            = 2008, // 시스템, 명중 확률
    System_Dodge          = 2009, // 시스템, 회피 확률

    // System_Damage_Reduce  = 2002, // 시스템, 데미지 감소
    //System_Damage_Magic   = 2002, // 시스템, 마법 데미지
    // System_Dodge_Critical = 2007, // 시스템, 필살 회피 확률


    System_TurnSequence   = 2100, // 시스템, 행동 순서 
    System_TurnCount      = 2101, // 시스템, 행동 횟수 
    System_AttackCount    = 2102, // 시스템, 공격 횟수 (행동당)

    System_Invincibility  = 2201, // 시스템, 무적
    System_Evasion        = 2202, // 시스템, 회피
}


public enum EnumBuffOption
{
    LimitOfUse,  // 횟수 제한
    LimitOfTurn, // 턴 제한
    Immutable,   // 변경 불가능(해제 불가능)

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
        return new BuffTarget { Situation = (int)_situation, Target = (int)_target, Status = (int)_status };
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

    
    public void Decrease_LimitOfUse()
    {
        // 횟 수 제한이 있다면 값을 깍아줍시다.
        if (HasValue(EnumBuffOption.LimitOfUse))
        {
            DecreaseValue(EnumBuffOption.LimitOfUse);
        }
    }

    public void Decrease_LimitOfTurn()
    {
        // 턴 수 제한이 있다면 값을 깍아줍시다.
        if (HasValue(EnumBuffOption.LimitOfTurn))
        {
            DecreaseValue(EnumBuffOption.LimitOfTurn);
        }
    }
}


public struct Buff
{
    public long                 ID;
    public BuffTarget           Target;
    public BuffValue            Value;
    public BuffOption           Option;
    public EnumBuffContentsType ContentsType;
    // public List<ICondition>     Conditions;    
           

    public readonly static Buff Empty = new Buff
    {
        ID           = 0,
        Target       = BuffTarget.Empty,
        Value        = BuffValue.Empty,
        Option       = BuffOption.Empty,
        ContentsType = EnumBuffContentsType.None
        // Conditions   = null,
    };

    public bool IsValidCondition(IOwner _owner)
    {
        // if (Conditions != null)
        // {
        //     foreach (var e in Conditions)
        //     {
        //         if (e != null && !e.IsValid(_owner))
        //             return false;
        //     }
        // }

        return true;
    }

    public bool IsExpired()
    {
        return Option.IsExpired();
    }

    static public Buff Create(
        Int64                _id,
        EnumBuffContentsType _contents_type,
        BuffTarget           _target,
        BuffValue            _value,
        BuffOption           _option
    )
    {
        return new Buff()
        {
            ID           = _id,
            ContentsType = _contents_type,
            Target       = _target,
            Value        = _value,
            Option       = _option
        };
    }

    static public Buff CreateBuff(Int32 _kind, EnumBuffContentsType _contents_type)
    {
        return Buff.Create(
            _kind, 
            _contents_type,
            DataManager.Instance.BuffSheet.GetBuffTarget(_kind),
            DataManager.Instance.BuffSheet.GetBuffValue(_kind),
            BuffOption.Empty
        );
    }
}

public class BuffMananger //: IBuff
{



    Dictionary<long, Buff>                          m_list_buff                     = new Dictionary<long, Buff>();
    Dictionary<BuffTarget, HashSet<long>>           m_list_buff_id_by_target        = new Dictionary<BuffTarget, HashSet<long>>();
    Dictionary<EnumBuffContentsType, HashSet<long>> m_list_buff_id_by_contents_type = new Dictionary<EnumBuffContentsType, HashSet<long>>();


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
        {
            if (!m_list_buff_id_by_target.TryGetValue(_buff.Target, out var list_buff_id))
            {
                list_buff_id = new HashSet<long>();
                m_list_buff_id_by_target.Add(_buff.Target, list_buff_id);
            }
            list_buff_id.Add(_buff.ID);
        }

        // 콘텐츠 타입에 등록
        {
            if (!m_list_buff_id_by_contents_type.TryGetValue(_buff.ContentsType, out var list_buff_id))
            {
                list_buff_id = new HashSet<long>();
                m_list_buff_id_by_contents_type.Add(_buff.ContentsType, list_buff_id);
            }
            list_buff_id.Add(_buff.ID);
        }


        return true;
    }

    public Buff      GetBuff(long _id) => m_list_buff.TryGetValue(_id, out var node) ? node : Buff.Empty;


    public void RemoveBuff(long _id)
    {
        var buff = GetBuff(_id);

        m_list_buff.Remove(_id);

        {
            if (m_list_buff_id_by_target.TryGetValue(buff.Target, out var list_buff_id))
                list_buff_id.Remove(buff.ID);
        }

        {
            if (m_list_buff_id_by_contents_type.TryGetValue(buff.ContentsType, out var list_buff_id))
                list_buff_id.Remove(buff.ID);
        }
    }

    private HashSet<Int64> GetBuffIDList(BuffTarget _target)
    {
        if (m_list_buff_id_by_target.TryGetValue(_target, out var list_buff_id))
        {
            return list_buff_id;
        }
        return null;
    }

    public BuffValue Collect_BuffValue(IOwner _owner, BuffTarget _target)
    {
        var result = BuffValue.Empty;

        using var list_buff_id = HashSetPool<Int64>.AcquireWrapper();

        // 
        var target      = GetBuffIDList(_target);

        // none situation 도 적용.
        var target_none = (_target.Situation != (int)EnumSituationType.None) 
                        ? GetBuffIDList(BuffTarget.Create(EnumSituationType.None,
                                       (EnumBuffTarget)_target.Target, 
                                       (EnumBuffStatus)_target.Status)) 
                        : null; 

        if (target      != null) list_buff_id.Value.UnionWith(target);
        if (target_none != null) list_buff_id.Value.UnionWith(target_none);       
        
       
        foreach (var id in list_buff_id.Value)
        {
            var buff = GetBuff(id);
            if (buff.IsExpired())
                continue;

            if (!buff.IsValidCondition(_owner))
                continue;

            result += buff.Value;
            
            // 사용 횟 수 깍기
            buff.Option.Decrease_LimitOfUse();                                        
        }

        return result;
    }


        


    // #region IBuff Interface





    public BuffValue Collect_Combat(
        EnumSituationType _situation,
        IOwner _attacker,
        IOwner _defender,
        EnumBuffStatus _status) 
    {
        var result = BuffValue.Empty;

        // 공격자 버프 계산.
        result += Collect(_situation, _attacker, _status);

        // 피격자 버프 계산.
        result += CollectTarget(_situation, _defender, _status);

        return result;
    }

    public BuffValue Collect(EnumSituationType _situation, IOwner _owner, EnumBuffStatus _status) 
    {
        return Collect_BuffValue(_owner, BuffTarget.Create(_situation, EnumBuffTarget.Owner, _status));
    }

    public BuffValue CollectTarget(EnumSituationType _situation, IOwner _owner, EnumBuffStatus _status) 
    {
        return Collect_BuffValue(_owner, BuffTarget.Create(_situation, EnumBuffTarget.Target, _status));
    }


    public void Collect_BuffID_ByContentsType(EnumBuffContentsType _contents_type, List<Int64> _list_buff_id)
    {
        if (m_list_buff_id_by_contents_type.TryGetValue(_contents_type, out var list_buff_id))
        {
            _list_buff_id.AddRange(list_buff_id);
        }
    }


    



    // #endregion IBuff Interface

    public BuffManager_IO Save()
    {
        return new BuffManager_IO()
        {
            Buffs = new List<Buff>(m_list_buff.Values)
        };
    }

    public void Load(BuffManager_IO _snapshot)
    {
        m_list_buff.Clear();
        m_list_buff_id_by_target.Clear();
        m_list_buff_id_by_contents_type.Clear();

        foreach (var buff in _snapshot.Buffs)
        {
            AddBuff(buff);
        }
    }


}

public class BuffManager_IO
{
    public List<Buff> Buffs { get; set; } = new();
}


