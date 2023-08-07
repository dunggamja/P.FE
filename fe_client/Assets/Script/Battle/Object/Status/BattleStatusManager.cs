using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class BattleStatusManager
    {
        public IUnit          Unit          { get; }
        public IWeapon        Weapon        { get; }
        public ITerrain       Terrain       { get; }
        public IBuff          Buff          { get; }
        //public IBlackBoard    BlackBoard    { get; }
        //public IActionCounter ActionCounter { get; }

        /// <summary> ���� ���� </summary>
        public int Calc_Might_Physic()
        {
            var unit_might   = Unit.GetStatus(EnumUnitStatus.Strength);
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might);

            unit_might       = Buff.Calculate(this, EnumBuffStatus.Unit_Strength).Calculate(unit_might);
            weapon_might     = Buff.Calculate(this, EnumBuffStatus.Weapon_Might).Calculate(weapon_might);
            
            return unit_might + weapon_might;
        }

        /// <summary> ���� ���� </summary>
        public int Calc_Might_Magic()
        {
            var unit_might   = Unit.GetStatus(EnumUnitStatus.Magic);
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might);

            unit_might       = Buff.Calculate(this, EnumBuffStatus.Unit_Magic).Calculate(unit_might);
            weapon_might     = Buff.Calculate(this, EnumBuffStatus.Weapon_Might).Calculate(weapon_might);

            return unit_might + weapon_might;
        }

        /// <summary> ���� </summary>
        public int Calc_Hit()
        {
            var unit_skill = Unit.GetStatus(EnumUnitStatus.Skill);
            var unit_luck  = Unit.GetStatus(EnumUnitStatus.Luck);
            
            var unit_hit   = unit_skill * 2 + unit_luck / 2;
            var weapon_hit = Weapon.GetStatus(EnumWeaponStatus.Hit);

            return unit_hit + weapon_hit;
        }

        /// <summary> �ʻ� </summary>
        public int Calc_Critical()
        {
            var unit_skill      = Unit.GetStatus(EnumUnitStatus.Skill);

            var unit_critical   = unit_skill / 2;
            var weapon_critical = Weapon.GetStatus(EnumWeaponStatus.Critical);

            return unit_critical + weapon_critical;
        }


        /// <summary> ȸ�� </summary>
        public int Calc_Dodge()
        {
            var battle_speed = Calc_Speed();
            var unit_luck    = Unit.GetStatus(EnumUnitStatus.Luck);
            var weapon_dodge = Weapon.GetStatus(EnumWeaponStatus.Dodge);

            return battle_speed * 2 + unit_luck / 2 + weapon_dodge;
        }

        /// <summary> �ʻ� ȸ�� </summary>
        public int Calc_DodgeCritical()
        {
            var unit_luck             = Unit.GetStatus(EnumUnitStatus.Luck);
            var weapon_dodge_critical = Weapon.GetStatus(EnumWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        /// <summary> �ӵ� </summary>
        public int Calc_Speed()
        {
            var unit_speed   = Unit.GetStatus(EnumUnitStatus.Speed);
            var unit_weight  = Unit.GetStatus(EnumUnitStatus.Weight);
            var wepon_weight = Weapon.GetStatus(EnumWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - unit_weight);
        }

        /// <summary> ��� </summary>
        public int Calc_Defense()
        {
            var unit_defense = Unit.GetStatus(EnumUnitStatus.Defense);
            var unit_buff    = Buff.Calculate(this, EnumBuffStatus.Unit_Defense);

            return unit_defense;
        }

        /// <summary> ���� </summary>
        public int Calc_Resistance()
        {
            var unit_resistance = Unit.GetStatus(EnumUnitStatus.Resistance);
            var unit_buff       = Buff.Calculate(this, EnumBuffStatus.Unit_Resistance);

            unit_resistance     = unit_buff.Calculate(unit_resistance);

            return unit_resistance;
        }

        ///// <summary> �ൿ ����, (������ �ǹ̰� ����. ������ ���ؼ��� ������ �޴´�.) </summary>
        //public int Calc_ActionTurn(BattleStatusManager _attacker)
        //{
        //    return _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_ActionTurn).Calculate(0);
        //}

        ///// <summary> �ൿ Ƚ�� </summary>
        //public int Calc_ActionCount(BattleStatusManager _attacker)
        //{
        //    return _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_ActionTurn).Calculate(1);
        //}

        ///// <summary> ���� Ƚ�� </summary>
        //public int Calc_AttackCount(BattleStatusManager _attacker)
        //{
        //    return _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_ActionTurn).Calculate(1);
        //}

    }


    //public interface IBattleCalculator 
    //{
    //    int Calc_Damage(BattleObject _attacker, BattleObject _defender);
    //}


    //public class BattleCalculator_OnCombat //: IBattleCalculator
    //{ 
    //    //public const int DAMAGE_MULTIPLIER_CRITICAL = 3;  // ũ��Ƽ�� ������ ����
    //    //public const int DAMAGE_MULTIPLIER_EFFECT   = 2;  // Ưȿ ������ ���� (ex: Ȱ=>����)

    //    //public int Calc_Damage(BattleStatusManager _attacker, BattleStatusManager _defender)
    //    //{
    //    //    var attacker_physics    = _attacker.Calc_Might_Physic();
    //    //    var attacker_magic      = _attacker.Calc_Might_Magic();

    //    //    var defender_defense    = _defender.Calc_Defense();
    //    //    var defender_resistance = _defender.Calc_Resistance();


    //    //    var damage_physics = Math.Max(0, attacker_physics - defender_defense);
    //    //    var damage_magic   = Math.Max(0, attacker_magic   - defender_resistance);

    //    //    // ������ ���� ���.
    //    //    damage_physics = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Damage_Physic).Calculate(damage_physics);
    //    //    damage_magic   = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Damage_Magic).Calculate(damage_magic);

    //    //    // ����� ���� ���.
    //    //    damage_physics = _defender.Buff.Calculate(_defender, EnumBuffStatus.Battle_Damage_Physic).Calculate(damage_physics);
    //    //    damage_magic   = _defender.Buff.Calculate(_defender, EnumBuffStatus.Battle_Damage_Magic).Calculate(damage_magic);

    //    //    return damage_physics + damage_magic;
    //    //}


    //    //public EnumActionTurn Calc_ActionTurn_Next(BattleObject _attacker, BattleObject _defender)
    //    //{
    //    //    //// �ൿ���� ���� �� ���.
    //    //    //var attacker_turn = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_ActionTurn).Calculate(ACTION_TURN_START_VALUE);
    //    //    //var defender_turn = _defender.Buff.Calculate(_defender, EnumBuffStatus.Battle_ActionTurn).Calculate(ACTION_TURN_START_VALUE);

    //    //    //// �����ִ� ���� Ƚ�� ���� üũ.
    //    //    //{
    //    //    //    if (_attacker.ActionCounter.IsRemainAttackCount())
    //    //    //        return EnumActionTurn.Attacker;

    //    //    //    if (_defender.ActionCounter.IsRemainAttackCount())
    //    //    //        return EnumActionTurn.Defender;
    //    //    //}


    //    //    //// �����ִ� �ൿ Ƚ�� üũ.
    //    //    //var attacker_enable_action = _attacker.ActionCounter.IsRemainActionCount();
    //    //    //var defender_enable_action = _defender.ActionCounter.IsRemainActionCount();

    //    //    //// ������ ���� ���� üũ.
    //    //    //if (attacker_enable_action)            
    //    //    //{
    //    //    //    // �ൿ �ð� / �ൿ Ƚ�� ���� üũ.
    //    //    //    var is_attacker_action_time = _attacker.ActionCounter.GetActionTime() <= _defender.ActionCounter.GetActionTime();
    //    //    //    if (is_attacker_action_time || !defender_enable_action)
    //    //    //        return EnumActionTurn.Attacker;
    //    //    //}

    //    //    //// ����� ���� üũ.
    //    //    //if (defender_enable_action)
    //    //    //    return EnumActionTurn.Defender;


    //    //    //// �ൿ �� �� �ִ� �ο� ����.
    //    //    //return EnumActionTurn.None;
    //    //}

    //    //void BattleOrder_Initialize_OrderCount(BattleObject _attacker, BattleObject _defender)
    //    //{
    //    //    // ���� �ӵ��� ����մϴ�.
    //    //    var attacker_speed        = _attacker.Calc_Speed();
    //    //    var defender_speed        = _defender.Calc_Speed();

    //    //    attacker_speed            = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Speed).Calculate(attacker_speed);
    //    //    defender_speed            = _defender.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Speed).Calculate(defender_speed);

    //    //    // 2�� ���� ����.
    //    //    var attacker_attack_twice = (attacker_speed - defender_speed) >= ATTACK_FOLLOW_SPEED;
    //    //    var defender_attack_twice = (defender_speed - attacker_speed) >= ATTACK_FOLLOW_SPEED;
    //    //}
    //}

}



/*
 * ����[����]     = ��[����] + ����[������] ����*(1[1] or 2[2] or 3[3]) + ��Ÿ[A](��ų ����ȿ�� ��)
 * ����           = ���*2 + ���/2 + ����[������] ���� + ��Ÿ[A](��ų ����ȿ�� ��)
 * �ʻ�           = ���/2 + ����[������] �ʻ� + ��Ÿ[A]
 * ���ط�[����]    = (����[����] - ����[����])*(1 or 3[7])
 * ����[8]        = �ӵ� - (���� - ü��)[9]
 * ȸ��           = ����*2 + ���/2 + ���� ȸ�� + ��Ÿ[A]
 * �ʻ� ȸ��      = ��� + ���� �ʻ� ȸ�� + ��Ÿ[A]
 * ���߷�         = ���� - ȸ��
 * �ʻ� �߻� Ȯ��  = �ʻ� - �ʻ� ȸ��
 */

