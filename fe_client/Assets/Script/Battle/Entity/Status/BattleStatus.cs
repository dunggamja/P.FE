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
        public IStatus        Status  { get; private set; }
        public IBuff          Buff    { get; private set; }
        public IWeapon        Weapon  { get; private set; }
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

        public int GetBuffedWeaponStatus(IWeapon _weapon, EnumWeaponStatus _weapon_status, EnumSituationType _situation_type = EnumSituationType.None)
        {
            var status     = _weapon.GetStatus(_weapon_status);

            var buff_value = BuffValue.Empty;
            foreach(var e in BuffHelper.CollectBuff_WeaponStatus(_weapon_status))
            {
                buff_value += Buff.Collect(_situation_type, Owner, e);
            }

            return buff_value.Calculate(status);    
        }




        /// <summary> °ø°Ý·Â. </summary>
        public int Calc_Might_Physic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Physics);
            if (weapon_might <= 0)
            {
                // ¹«±â°¡ ¾øÀ¸¸é µ¥¹ÌÁöµµ ¾ø´Ù.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Strength);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Physics);
           
            return status_unit + status_weapon;
        }

        /// <summary> ¸¶¹ý°ø°Ý·Â. </summary>
        public int Calc_Might_Magic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Magic);
            if (weapon_might <= 0)
            {
                // ¹«±â°¡ ¾øÀ¸¸é µ¥¹ÌÁöµµ ¾ø´Ù.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Magic);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Magic);

            return status_unit + status_weapon;
        }

        /// <summary> ¸íÁß </summary>
        public int Calc_Hit()
        {
            var unit_skill = GetBuffedUnitStatus(EnumUnitStatus.Skill);
            var unit_luck  = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            
            var unit_hit   = unit_skill * 2 + unit_luck / 2;
            var weapon_hit = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Hit);

            return unit_hit + weapon_hit;
        }

        /// <summary> Ä¡¸í </summary>
        public int Calc_Critical()
        {
            var unit_skill      = GetBuffedUnitStatus(EnumUnitStatus.Skill);

            var unit_critical   = unit_skill / 2;
            var weapon_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Critical);

            return unit_critical + weapon_critical;
        }


        /// <summary> È¸ÇÇ </summary>
        public int Calc_Dodge()
        {
            var battle_speed = Calc_Speed();
            var unit_luck    = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge);

            return battle_speed * 2 + unit_luck / 2 + weapon_dodge;
        }

        /// <summary> Ä¡¸í È¸ÇÇ </summary>
        public int Calc_DodgeCritical()
        {
            var unit_luck             = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        /// <summary> ¼Óµµ </summary>
        public int Calc_Speed()
        {
            var unit_speed   = GetBuffedUnitStatus(EnumUnitStatus.Speed);
            var unit_weight  = GetBuffedUnitStatus(EnumUnitStatus.Weight);
            var wepon_weight = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - unit_weight);
        }

        /// <summary> ¹æ¾î·Â </summary>
        public int Calc_Defense()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Defense);
        }

        /// <summary> ¸¶¹ý¹æ¾î·Â </summary>
        public int Calc_Resistance()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Resistance);
        }
    }

}

/*
 ÆÄ¿¥ if ³ª¹«À§Å°º¸°í ´É·ÂÄ¡ °ø½Ä ±Ü¾î¿Âµí... ÀÎÄÚµùÀÌ ƒÆÁ®¹ö·È³×...
 * ï¿½ï¿½ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½]     = ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½] + ï¿½ï¿½ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½] ï¿½ï¿½ï¿½ï¿½*(1[1] or 2[2] or 3[3]) + ï¿½ï¿½Å¸[A](ï¿½ï¿½Å³ ï¿½ï¿½ï¿½ï¿½È¿ï¿½ï¿½ ï¿½ï¿½)
 * ï¿½ï¿½ï¿½ï¿½           = ï¿½ï¿½ï¿?*2 + ï¿½ï¿½ï¿?/2 + ï¿½ï¿½ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½] ï¿½ï¿½ï¿½ï¿½ + ï¿½ï¿½Å¸[A](ï¿½ï¿½Å³ ï¿½ï¿½ï¿½ï¿½È¿ï¿½ï¿½ ï¿½ï¿½)
 * ï¿½Ê»ï¿½           = ï¿½ï¿½ï¿?/2 + ï¿½ï¿½ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½] ï¿½Ê»ï¿½ + ï¿½ï¿½Å¸[A]
 * ï¿½ï¿½ï¿½Ø·ï¿½         = (ï¿½ï¿½ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½] - ï¿½ï¿½ï¿½ï¿½[ï¿½ï¿½ï¿½ï¿½])*(1 or 3[7])
 * ï¿½ï¿½ï¿½ï¿½[8]        = ï¿½Óµï¿½ - (ï¿½ï¿½ï¿½ï¿½ - Ã¼ï¿½ï¿½)[9]
 * È¸ï¿½ï¿½           = ï¿½ï¿½ï¿½ï¿½*2 + ï¿½ï¿½ï¿?/2 + ï¿½ï¿½ï¿½ï¿½ È¸ï¿½ï¿½ + ï¿½ï¿½Å¸[A]
 * ï¿½Ê»ï¿½ È¸ï¿½ï¿½      = ï¿½ï¿½ï¿? + ï¿½ï¿½ï¿½ï¿½ ï¿½Ê»ï¿½ È¸ï¿½ï¿½ + ï¿½ï¿½Å¸[A]
 * ï¿½ï¿½ï¿½ß·ï¿½         = ï¿½ï¿½ï¿½ï¿½ - È¸ï¿½ï¿½
 * ï¿½Ê»ï¿½ ï¿½ß»ï¿½ È®ï¿½ï¿½  = ï¿½Ê»ï¿½ - ï¿½Ê»ï¿½ È¸ï¿½ï¿½
 */

