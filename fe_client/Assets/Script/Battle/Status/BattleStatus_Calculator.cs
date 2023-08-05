using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleStatus
{
    

    public interface IUnit
    {
        int       GetStatus(EnumBattleUnitStatus _status_type);
        bool      HasAttribute(EnumBattleUnitAttribute _attribute_type);
        //BuffValue Buff(EnumBuffGroup _group, EnumBuffSituation _situation, EnumBuffTarget _target, EnumBuffStatus _status);
    }

    public interface IWeapon
    {
        int       GetStatus(EnumBattleWeaponStatus _status_type);
        bool      HasAttribute(EnumBattleWeaponAttribute _attribute_type);
        //BuffValue Buff(EnumBuffGroup _group, EnumBuffSituation _situation, EnumBuffTarget _target, EnumBuffStatus _status);
    }

    public interface ITerrain
    {
        int       GetStatus(EnumBattleTerrainStatus _status_type);
        bool      HasAttribute(EnumBattleTerrainAttribute _attribute_type);
        //BuffValue Buff(EnumBuffGroup _group, EnumBuffSituation _situation, EnumBuffTarget _target, EnumBuffStatus _status);
    }

    public interface IBuff
    {
        BuffValue Calculate(BattleStatusOwner _owner, EnumBuffStatus _status/*, params EnumBuffBattleSituation[] _situation*/);
        //BuffValue Buff_System(BattleStatusOwner _owner, EnumBuffStatus _status/*, params EnumBuffBattleSituation[] _situation*/);
    }

    public interface ICondition
    {
        bool IsValid(BattleStatusOwner _owner);
    }

    public interface IEffect
    {
        void Apply(BattleStatusOwner _owner);
    }

    public interface IBlackBoard
    {
        int  GetValue(EnumBattleBlackBoard _type);
        bool HasValue(EnumBattleBlackBoard _type);
        void SetValue(EnumBattleBlackBoard _type, int  _value);
        void SetValue(EnumBattleBlackBoard _type, bool _value);
    }


    public abstract class BattleStatusOwner
    {
        public IUnit       Unit       { get; }
        public IWeapon     Weapon     { get; }
        public ITerrain    Terrain    { get; }
        public IBuff       Buff       { get; }
        public IBlackBoard BlackBoard { get; }


        public int Calc_Might_Physic()
        {
            var unit_might   = Unit.GetStatus(EnumBattleUnitStatus.Strength);
            var weapon_might = Weapon.GetStatus(EnumBattleWeaponStatus.Might);

            var unit_buff    = Buff.Calculate(this, EnumBuffStatus.Unit_Strength);
            var weapon_buff  = Buff.Calculate(this, EnumBuffStatus.Weapon_Might);

            unit_might      = unit_buff.Calculate(unit_might);
            weapon_might    = weapon_buff.Calculate(weapon_might);
            
            return unit_might + weapon_might;
        }

        public int Calc_Might_Magic()
        {
            var unit_might   = Unit.GetStatus(EnumBattleUnitStatus.Magic);
            var weapon_might = Weapon.GetStatus(EnumBattleWeaponStatus.Might);

            var unit_buff    = Buff.Calculate(this, EnumBuffStatus.Unit_Magic);
            var weapon_buff  = Buff.Calculate(this, EnumBuffStatus.Weapon_Might);

            unit_might       = unit_buff.Calculate(unit_might);
            weapon_might     = weapon_buff.Calculate(weapon_might);

            return unit_might + weapon_might;
        }


        public int Calc_Hit()
        {
            var unit_skill = Unit.GetStatus(EnumBattleUnitStatus.Skill);
            var unit_luck  = Unit.GetStatus(EnumBattleUnitStatus.Luck);
            
            var unit_hit   = unit_skill * 2 + unit_luck / 2;
            var weapon_hit = Weapon.GetStatus(EnumBattleWeaponStatus.Hit);

            return unit_hit + weapon_hit;
        }

        public int Calc_Critical()
        {
            var unit_skill      = Unit.GetStatus(EnumBattleUnitStatus.Skill);

            var unit_critical   = unit_skill / 2;
            var weapon_critical = Weapon.GetStatus(EnumBattleWeaponStatus.Critical);

            return unit_critical + weapon_critical;
        }



        public int Calc_Dodge()
        {
            var battle_speed = Calc_Speed();
            var unit_luck    = Unit.GetStatus(EnumBattleUnitStatus.Luck);
            var weapon_dodge = Weapon.GetStatus(EnumBattleWeaponStatus.Dodge);

            return battle_speed * 2 + unit_luck / 2 + weapon_dodge;
        }

        public int Calc_DodgeCritical()
        {
            var unit_luck             = Unit.GetStatus(EnumBattleUnitStatus.Luck);
            var weapon_dodge_critical = Weapon.GetStatus(EnumBattleWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        public int Calc_Speed()
        {
            var unit_speed   = Unit.GetStatus(EnumBattleUnitStatus.Speed);
            var unit_weight  = Unit.GetStatus(EnumBattleUnitStatus.Weight);
            var wepon_weight = Weapon.GetStatus(EnumBattleWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - unit_weight);
        }

        public int Calc_Defense()
        {
            var unit_defense = Unit.GetStatus(EnumBattleUnitStatus.Defense);
            var unit_buff    = Buff.Calculate(this, EnumBuffStatus.Unit_Defense);

            return unit_defense;
        }

        public int Calc_Resistance()
        {
            var unit_resistance = Unit.GetStatus(EnumBattleUnitStatus.Resistance);
            var unit_buff       = Buff.Calculate(this, EnumBuffStatus.Unit_Resistance);

            unit_resistance     = unit_buff.Calculate(unit_resistance);

            return unit_resistance;
        }
    }


    

    public interface IBattleCalculator 
    {
        int Calc_Damage(BattleStatusOwner _attacker, BattleStatusOwner _defender);
    }


    public class BattleCalculator_OnCombat : IBattleCalculator
    { 
        public const int DAMAGE_MULTIPLIER_CRITICAL = 3; // ũ��Ƽ�� ������ ����
        public const int DAMAGE_MULTIPLIER_EFFECT   = 2; // Ưȿ ������ ���� (ex: Ȱ=>����)
        public const int ATTACK_TWICE_SPEED         = 5; // �ӵ��� X�� �̻� ���̳��� 2�� ����.

        public int Calc_Damage(BattleStatusOwner _attacker, BattleStatusOwner _defender)
        {
            var attacker_physics    = _attacker.Calc_Might_Physic();
            var attacker_magic      = _attacker.Calc_Might_Magic();

            var defender_defense    = _defender.Calc_Defense();
            var defender_resistance = _defender.Calc_Resistance();


            var damage_physics = Math.Max(0, attacker_physics - defender_defense);
            var damage_magic   = Math.Max(0, attacker_magic   - defender_resistance);

            // ������ ���� ���.
            damage_physics = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Damage_Physic).Calculate(damage_physics);
            damage_magic   = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Damage_Magic).Calculate(damage_magic);

            // ����� ���� ���.
            damage_physics = _defender.Buff.Calculate(_defender, EnumBuffStatus.Battle_Damage_Physic).Calculate(damage_physics);
            damage_magic   = _defender.Buff.Calculate(_defender, EnumBuffStatus.Battle_Damage_Magic).Calculate(damage_magic);

            return damage_physics + damage_magic;
        }



        public List<EnumBattleTurn> Calc_BattleTurn(BattleStatusOwner _attacker, BattleStatusOwner _defender)
        {
            var list_turn = new List<EnumBattleTurn>();

            
            var attacker_speed = _attacker.Calc_Speed();
            var defender_speed = _defender.Calc_Speed();

            attacker_speed     = _attacker.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Speed).Calculate(attacker_speed);
            defender_speed     = _defender.Buff.Calculate(_attacker, EnumBuffStatus.Battle_Speed).Calculate(defender_speed);



            // 2�� ���� ����.
            var attacker_attack_twice = (attacker_speed - defender_speed) >= ATTACK_TWICE_SPEED;
            var defender_attack_twice = (defender_speed - attacker_speed) >= ATTACK_TWICE_SPEED;





            return list_turn;
        }

    }

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

