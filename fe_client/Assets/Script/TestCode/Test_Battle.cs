using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;

public class Test_Battle : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //}

    class TestBattleParam : ICombatSystemParam
    {
        BattleObject m_attacker;
        BattleObject m_defender;


        public BattleObject Attacker => m_attacker;

        public BattleObject Defender => m_defender;

        public bool IsPlan => false;

        public TestBattleParam(BattleObject _attacker, BattleObject _defender)
        {
            m_attacker = _attacker;
            m_defender = _defender;
        }
    }


    TestBattleParam Param;

    private void Start()
    {
        var attacker = BattleObject.Create();
        var defender = BattleObject.Create();

        // 테스트를 위한 임시 능력치 셋팅.
        var attacker_status = attacker.StatusManager.Status as UnitStatus;
        var defender_status = defender.StatusManager.Status as UnitStatus;

        var attacker_weapon = attacker.StatusManager.Weapon as Weapon;
        var defender_weapon = defender.StatusManager.Weapon as Weapon;

        attacker_status.SetPoint(EnumUnitPoint.HP, 22);
        attacker_status.SetStatus(EnumUnitStatus.Strength, 6);
        attacker_status.SetStatus(EnumUnitStatus.Skill, 5);
        attacker_status.SetStatus(EnumUnitStatus.Speed, 7);
        attacker_status.SetStatus(EnumUnitStatus.Movement, 4);
        attacker_status.SetStatus(EnumUnitStatus.Defense, 5);
        attacker_status.SetStatus(EnumUnitStatus.Resistance, 3);
        attacker_status.SetStatus(EnumUnitStatus.Luck, 5);
        attacker_status.SetStatus(EnumUnitStatus.Weight, 4);


        defender_status.SetPoint(EnumUnitPoint.HP, 22);
        defender_status.SetStatus(EnumUnitStatus.Strength, 6);
        defender_status.SetStatus(EnumUnitStatus.Skill, 5);
        defender_status.SetStatus(EnumUnitStatus.Speed, 7);
        defender_status.SetStatus(EnumUnitStatus.Movement, 4);
        defender_status.SetStatus(EnumUnitStatus.Defense, 5);
        defender_status.SetStatus(EnumUnitStatus.Resistance, 3);
        defender_status.SetStatus(EnumUnitStatus.Luck, 5);
        defender_status.SetStatus(EnumUnitStatus.Weight, 4);


        attacker_weapon.SetStatus(EnumWeaponStatus.Might, 3);
        attacker_weapon.SetStatus(EnumWeaponStatus.Hit, 100);
        attacker_weapon.SetStatus(EnumWeaponStatus.Weight, 2);
        attacker_weapon.SetStatus(EnumWeaponStatus.Dodge_Critical, 30);
        attacker_weapon.SetStatus(EnumWeaponStatus.Range, 1);

        defender_weapon.SetStatus(EnumWeaponStatus.Might, 3);
        defender_weapon.SetStatus(EnumWeaponStatus.Hit, 100);
        defender_weapon.SetStatus(EnumWeaponStatus.Weight, 2);
        defender_weapon.SetStatus(EnumWeaponStatus.Dodge_Critical, 30);
        defender_weapon.SetStatus(EnumWeaponStatus.Range, 1);



        Param = new TestBattleParam(attacker, defender);
        CombatSystemManager.Instance.SetData(Param);
    }


    [ContextMenu("TickUpdate")]
    void TickUpdate()
    {
        //IBattleSystemParam
        CombatSystemManager.Instance.Update();
    }
}
