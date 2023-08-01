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
        BuffValue Filter_Status(EnumBuffStatus _status, params EnumBuffSituation[] _situation);
        BuffValue Filter_System(EnumBuffStatus _status, params EnumBuffSituation[] _situation);
    }


    public abstract class BattleStatusOwner
    {
        public IUnit    Unit    { get; }
        public IWeapon  Weapon  { get; }
        public ITerrain Terrain { get; }
        public IBuff    Buff    { get; }


        public int Calc_Might_Physic(EnumBuffSituation _situation)
        {
            var unit_might   = Unit.GetStatus(EnumBattleUnitStatus.Strength);
            var weapon_might = Weapon.GetStatus(EnumBattleWeaponStatus.Might);

            var unit_buff    = Buff.Filter_Status(EnumBuffStatus.Unit_Strength, EnumBuffSituation.None, _situation);
            var weapon_buff  = Buff.Filter_Status(EnumBuffStatus.Weapon_Might, EnumBuffSituation.None, _situation);

            unit_might      = unit_buff.Calculate(unit_might);
            weapon_might    = weapon_buff.Calculate(weapon_might);
            
            return unit_might + weapon_might;
        }

        public int Calc_Might_Magic()
        {
            var unit_might   = Unit.GetStatus(EnumBattleUnitStatus.Magic);
            var weapon_might = Weapon.GetStatus(EnumBattleWeaponStatus.Might);

            var unit_buff    = Buff.Filter_Status(EnumBuffStatus.Unit_Magic, EnumBuffSituation.None);
            var weapon_buff  = Buff.Filter_Status(EnumBuffStatus.Weapon_Might, EnumBuffSituation.None);

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
            var unit_buff    = Buff.Filter(EnumBuffGroup.Status, EnumBuffSituation.None, EnumBuffTarget.Owner, EnumBuffStatus.Unit_Defense);

            return unit_defense;
        }

        public int Calc_Resistance()
        {
            var unit_resistance = Unit.GetStatus(EnumBattleUnitStatus.Resistance);
            var unit_buff       = Buff.Filter(EnumBuffGroup.Status, EnumBuffSituation.None, EnumBuffTarget.Owner, EnumBuffStatus.Unit_Resistance);

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
        public const int DAMAGE_MULTIPLIER_CRITICAL = 3; // 크리티컬 데미지 배율
        public const int DAMAGE_MULTIPLIER_EFFECT   = 2; // 특효 데미지 배율 (ex: 활=>비행)

        public int Calc_Damage(BattleStatusOwner _attacker, BattleStatusOwner _defender)
        {
            var attacker_might_physics = _attacker.Calc_Might_Physic();
            var attacker_might_magic   = _attacker.Calc_Might_Magic();

            var defender_defense       = _defender.Calc_Defense();
            var defender_resistance    = _defender.Calc_Resistance();


            _attacker.Buff.Filter(EnumBuffGroup.Situation, EnumBuffSituation.OnAttack, EnumBuffTarget.Owner, EnumBuffStatus.)

            //_defender.

            //_attacker.Buff.Filter_Buff(EnumBuffGroup.Situation)
        }



    }

}



/*
 * 공격[마공]     = 힘[마력] + 무기[마도서] 위력*(1[1] or 2[2] or 3[3]) + 기타[A](스킬 지형효과 등)
 * 명중           = 기술*2 + 행운/2 + 무기[마도서] 명중 + 기타[A](스킬 지형효과 등)
 * 필살           = 기술/2 + 무기[마도서] 필살 + 기타[A]
 * 피해량[마법]    = (공격[마공] - 수비[마방])*(1 or 3[7])
 * 공속[8]        = 속도 - (무게 - 체격)[9]
 * 회피           = 공속*2 + 행운/2 + 무기 회피 + 기타[A]
 * 필살 회피      = 행운 + 무기 필살 회피 + 기타[A]
 * 명중률         = 명중 - 회피
 * 필살 발생 확률  = 필살 - 필살 회피
 */


//public interface IBattleCalculator
//{
//    int Calc_Might_Physic(IBattleStatusOwner _param);
//    int Calc_Might_Magic(IBattleStatusOwner _param);
//    int Calc_Hit(IBattleStatusOwner _param);
//    int Calc_Critical(IBattleStatusOwner _param);
//    int Calc_Dodge(IBattleStatusOwner _param);
//    int Calc_DodgeCritical(IBattleStatusOwner _param);
//    int Calc_Speed(IBattleStatusOwner _param);
//}

