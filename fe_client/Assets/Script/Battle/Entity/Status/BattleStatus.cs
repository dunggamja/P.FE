using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    
    


    public class BattleStatusManager 
    {
        Int64 m_id = 0;

        public Int64          ID      => m_id;
        public IOwner         Owner   { get; private set; }
        public UnitStatus     Status  { get; private set; }
        public IBuff          Buff    { get; private set; }
        public Weapon         Weapon  { get; private set; }
        public ITerrain       Terrain { get; private set; }
        //public IBlackBoard    BlackBoard    { get; }
        //public IActionCounter ActionCounter { get; }

        public BattleStatusManager(Entity _owner)
        {
            m_id    = _owner.ID;
            Owner   = _owner;
            Status  = new UnitStatus();
            Buff    = new BuffMananger();
            Weapon  = new Weapon(_owner.ID, 0);
            Terrain = null;
        }

        public int GetBuffedUnitStatus(EnumUnitStatus _unit_status, EnumSituationType _situation_type = EnumSituationType.None)
        {
            var status     = Status.GetStatus(_unit_status);

            var buff_value = BuffValue.Empty;
            foreach(var e in BuffHelper.CollectBuff_UnitStatus(_unit_status))
            {
                buff_value += Buff.Collect(_situation_type, Owner, e);
            }
            
            
            return buff_value.Calculate(status);
        }

        public int GetBuffedWeaponStatus(Weapon _weapon, EnumWeaponStatus _weapon_status, EnumSituationType _situation_type = EnumSituationType.None)
        {
            var status     = _weapon.GetStatus(_weapon_status);

            var buff_value = BuffValue.Empty;
            foreach(var e in BuffHelper.CollectBuff_WeaponStatus(_weapon_status))
            {
                buff_value += Buff.Collect(_situation_type, Owner, e);
            }

            return buff_value.Calculate(status);    
        }




        /// <summary> 공격력. </summary>
        public int Calc_Might_Physic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Physics);
            if (weapon_might <= 0)
            {
                // 무기가 없으면 데미지도 없다.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Strength);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Physics);
           
            return status_unit + status_weapon;
        }

        /// <summary> 마법공격력. </summary>
        public int Calc_Might_Magic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Magic);
            if (weapon_might <= 0)
            {
                // 무기가 없으면 데미지도 없다.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Magic);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Magic);

            return status_unit + status_weapon;
        }

        /// <summary> 명중 </summary>
        public int Calc_Hit()
        {
            var unit_skill = GetBuffedUnitStatus(EnumUnitStatus.Skill);
            var unit_luck  = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            
            var unit_hit   = unit_skill * 2 + unit_luck / 2;
            var weapon_hit = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Hit);

            return unit_hit + weapon_hit;
        }

        /// <summary> 치명 </summary>
        public int Calc_Critical()
        {
            var unit_skill      = GetBuffedUnitStatus(EnumUnitStatus.Skill);

            var unit_critical   = unit_skill / 2;
            var weapon_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Critical);

            return unit_critical + weapon_critical;
        }


        /// <summary> 회피 </summary>
        public int Calc_Dodge()
        {
            var battle_speed = Calc_Speed();
            var unit_luck    = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge);

            return battle_speed * 2 + unit_luck / 2 + weapon_dodge;
        }

        /// <summary> 치명 회피 </summary>
        public int Calc_DodgeCritical()
        {
            var unit_luck             = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        /// <summary> 속도 </summary>
        public int Calc_Speed()
        {
            var unit_speed   = GetBuffedUnitStatus(EnumUnitStatus.Speed);
            var unit_weight  = GetBuffedUnitStatus(EnumUnitStatus.Weight);
            var wepon_weight = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - unit_weight);
        }

        /// <summary> 방어력 </summary>
        public int Calc_Defense()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Defense);
        }

        /// <summary> 마법방어력 </summary>
        public int Calc_Resistance()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Resistance);
        }
    }

}

/*
 파엠 if 나무위키보고 능력치 공식 긁어온듯... 인코딩이 꺠져버렸네...
 * 占쏙옙占쏙옙[占쏙옙占쏙옙]     = 占쏙옙[占쏙옙占쏙옙] + 占쏙옙占쏙옙[占쏙옙占쏙옙占쏙옙] 占쏙옙占쏙옙*(1[1] or 2[2] or 3[3]) + 占쏙옙타[A](占쏙옙킬 占쏙옙占쏙옙효占쏙옙 占쏙옙)
 * 占쏙옙占쏙옙           = 占쏙옙占?*2 + 占쏙옙占?/2 + 占쏙옙占쏙옙[占쏙옙占쏙옙占쏙옙] 占쏙옙占쏙옙 + 占쏙옙타[A](占쏙옙킬 占쏙옙占쏙옙효占쏙옙 占쏙옙)
 * 占십삼옙           = 占쏙옙占?/2 + 占쏙옙占쏙옙[占쏙옙占쏙옙占쏙옙] 占십삼옙 + 占쏙옙타[A]
 * 占쏙옙占쌔뤄옙         = (占쏙옙占쏙옙[占쏙옙占쏙옙] - 占쏙옙占쏙옙[占쏙옙占쏙옙])*(1 or 3[7])
 * 占쏙옙占쏙옙[8]        = 占쌈듸옙 - (占쏙옙占쏙옙 - 체占쏙옙)[9]
 * 회占쏙옙           = 占쏙옙占쏙옙*2 + 占쏙옙占?/2 + 占쏙옙占쏙옙 회占쏙옙 + 占쏙옙타[A]
 * 占십삼옙 회占쏙옙      = 占쏙옙占? + 占쏙옙占쏙옙 占십삼옙 회占쏙옙 + 占쏙옙타[A]
 * 占쏙옙占쌩뤄옙         = 占쏙옙占쏙옙 - 회占쏙옙
 * 占십삼옙 占쌩삼옙 확占쏙옙  = 占십삼옙 - 占십삼옙 회占쏙옙
 */

