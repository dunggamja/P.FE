using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CutsceneCondition_Combat_Unit : CutsceneCondition
{
    public TAG_INFO UnitTag { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);

    public CutsceneCondition_Combat_Unit(TAG_INFO _unit_tag)
    {
        UnitTag = _unit_tag;
    }

    public override bool Verify(CutsceneSequence _sequence)
    {
        // 공격자 ID, 방어자 ID
        var attacker_id = CombatSystemManager.Instance.AttackerID;
        var defender_id = CombatSystemManager.Instance.DefenderID;


        // 대상 유닛을 컬렉트 합니다.
        using var list_unit = ListPool<Entity>.AcquireWrapper();        
        TagHelper.Collect_Entity(UnitTag, list_unit.Value);

        // 공격 또는 방어유닛에 대상 유닛이 존재하는지 체크합니다.
        foreach(var e in list_unit.Value)
        {
            if (e.ID == attacker_id || e.ID == defender_id)
            {
                return true;
            }
        }

        return false;
    }
}


public class CutsceneCondition_Combat_Unit_Dead : CutsceneCondition
{
    public TAG_INFO UnitTag { get; private set; } = TAG_INFO.Create(EnumTagType.None, 0);

    public CutsceneCondition_Combat_Unit_Dead(TAG_INFO _unit_tag)
    {
        UnitTag = _unit_tag;
    }

    public override bool Verify(CutsceneSequence _sequence)
    {
        // 공격자 ID, 방어자 ID
        var attacker_id = CombatSystemManager.Instance.AttackerID;
        var defender_id = CombatSystemManager.Instance.DefenderID;



        using var list_unit = ListPool<Entity>.AcquireWrapper();        
        TagHelper.Collect_Entity(UnitTag, list_unit.Value);

        // 죽은 유닛 체크.
        foreach(var e in list_unit.Value)
        {
            if (e.ID == attacker_id || e.ID == defender_id)
            {                
                if (e.IsDead)
                    return true;
            }

        }
        return false;
    }
}
