using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class UnitStatus : IStatus
    {
        BaseContainer m_point_repository     = new BaseContainer();
        BaseContainer m_status_repository    = new BaseContainer();
        BaseContainer m_attribute_repository = new BaseContainer();

        public int GetPoint(EnumUnitPoint _point_type)
        {
            return m_point_repository.GetValue((int)_point_type);
        }

        public int GetStatus(EnumUnitStatus _status_type)
        {
            return m_status_repository.GetValue((int)_status_type);
        }

        public bool HasAttribute(EnumUnitAttribute _attribute_type)
        {
            return m_attribute_repository.HasValue((int)_attribute_type);
        }

        public void SetPoint(EnumUnitPoint _point_type, int _value)
        {
            m_point_repository.SetValue((int)_point_type, _value);
        }

        public void SetStatus(EnumUnitStatus _status_type, int _value)
        {
            m_status_repository.SetValue((int)_status_type, _value);
        }

        public void SetAttribute(EnumUnitAttribute _attribute_type, bool _value)
        {
            m_attribute_repository.SetValue((int)_attribute_type, _value);
        }

    }


    public class BattleStatusManager 
    {
        Int64 m_id = 0;

        public Int64          ID      => m_id;
        public IOwner         Owner   { get; private set; }
        public IStatus        Status  { get; private set; }
        public IWeapon        Weapon  { get; private set; }
        public IBuff          Buff    { get; private set; }
        public ITerrain       Terrain { get; private set; }
        //public IBlackBoard    BlackBoard    { get; }
        //public IActionCounter ActionCounter { get; }

        public BattleStatusManager(BattleObject _owner)
        {
            m_id    = _owner.ID;
            Owner   = _owner;
            Status  = new UnitStatus();
            Weapon  = new Weapon();
            Buff    = new BuffMananger();
            Terrain = null;
        }




        /// <summary> 물리 위력 </summary>
        public int Calc_Might_Physic()
        {
            var unit_might   = Status.GetStatus(EnumUnitStatus.Strength);
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might);

            unit_might       = Buff.Collect(null, Owner, EnumBuffStatus.Unit_Strength).Calculate(unit_might);
            weapon_might     = Buff.Collect(null, Owner, EnumBuffStatus.Weapon_Might).Calculate(weapon_might);
            
            return unit_might + weapon_might;
        }

        /// <summary> 마법 위력 </summary>
        public int Calc_Might_Magic()
        {
            var unit_might   = Status.GetStatus(EnumUnitStatus.Magic);
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might);

            unit_might       = Buff.Collect(null, Owner, EnumBuffStatus.Unit_Magic).Calculate(unit_might);
            weapon_might     = Buff.Collect(null, Owner, EnumBuffStatus.Weapon_Might).Calculate(weapon_might);

            return unit_might + weapon_might;
        }

        /// <summary> 명중 </summary>
        public int Calc_Hit()
        {
            var unit_skill = Status.GetStatus(EnumUnitStatus.Skill);
            var unit_luck  = Status.GetStatus(EnumUnitStatus.Luck);
            
            var unit_hit   = unit_skill * 2 + unit_luck / 2;
            var weapon_hit = Weapon.GetStatus(EnumWeaponStatus.Hit);



            return unit_hit + weapon_hit;
        }

        /// <summary> 필살 </summary>
        public int Calc_Critical()
        {
            var unit_skill      = Status.GetStatus(EnumUnitStatus.Skill);

            var unit_critical   = unit_skill / 2;
            var weapon_critical = Weapon.GetStatus(EnumWeaponStatus.Critical);

            return unit_critical + weapon_critical;
        }


        /// <summary> 회피 </summary>
        public int Calc_Dodge()
        {
            var battle_speed = Calc_Speed();
            var unit_luck    = Status.GetStatus(EnumUnitStatus.Luck);
            var weapon_dodge = Weapon.GetStatus(EnumWeaponStatus.Dodge);

            return battle_speed * 2 + unit_luck / 2 + weapon_dodge;
        }

        /// <summary> 필살 회피 </summary>
        public int Calc_DodgeCritical()
        {
            var unit_luck             = Status.GetStatus(EnumUnitStatus.Luck);
            var weapon_dodge_critical = Weapon.GetStatus(EnumWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        /// <summary> 속도 </summary>
        public int Calc_Speed()
        {
            var unit_speed   = Status.GetStatus(EnumUnitStatus.Speed);
            var unit_weight  = Status.GetStatus(EnumUnitStatus.Weight);
            var wepon_weight = Weapon.GetStatus(EnumWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - unit_weight);
        }

        /// <summary> 방어 </summary>
        public int Calc_Defense()
        {
            var unit_defense = Status.GetStatus(EnumUnitStatus.Defense);
            var unit_buff    = Buff.Collect(null, Owner, EnumBuffStatus.Unit_Defense);

            return unit_defense;
        }

        /// <summary> 마방 </summary>
        public int Calc_Resistance()
        {
            var unit_resistance = Status.GetStatus(EnumUnitStatus.Resistance);
            var unit_buff       = Buff.Collect(null, Owner, EnumBuffStatus.Unit_Resistance);

            unit_resistance     = unit_buff.Calculate(unit_resistance);

            return unit_resistance;
        }
    }

}

/*
 * 공격[마공]     = 힘[마력] + 무기[마도서] 위력*(1[1] or 2[2] or 3[3]) + 기타[A](스킬 지형효과 등)
 * 명중           = 기술*2 + 행운/2 + 무기[마도서] 명중 + 기타[A](스킬 지형효과 등)
 * 필살           = 기술/2 + 무기[마도서] 필살 + 기타[A]
 * 피해량         = (공격[마공] - 수비[마방])*(1 or 3[7])
 * 공속[8]        = 속도 - (무게 - 체격)[9]
 * 회피           = 공속*2 + 행운/2 + 무기 회피 + 기타[A]
 * 필살 회피      = 행운 + 무기 필살 회피 + 기타[A]
 * 명중률         = 명중 - 회피
 * 필살 발생 확률  = 필살 - 필살 회피
 */

