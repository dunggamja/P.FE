using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    
    


    public class StatusManager 
    {
        // Int64 m_id = 0;

        // public Int64          ID      => m_id;
        public IOwner         Owner   { get; private set; }
        public UnitStatus     Status  { get; private set; }
        public BuffMananger   Buff    { get; private set; }
        public Weapon         Weapon  { get; private set; }
        // public ITerrain       Terrain { get; private set; }
        //public IBlackBoard    BlackBoard    { get; }
        //public IActionCounter ActionCounter { get; }

        public StatusManager(Entity _owner)
        {
            // m_id    = _owner.ID;
            Owner   = _owner;
            Status  = new UnitStatus();
            Buff    = new BuffMananger();
            Weapon  = new Weapon(_owner.ID, 0);
            // Terrain = null;
        }

        public int GetBuffedUnitStatus(EnumUnitStatus _unit_status, EnumSituationType _situation_type = EnumSituationType.None)
        {
            var status     = Status.GetStatus(_unit_status);

            var buff_value = BuffValue.Empty;
            foreach(var e in BuffHelper.CollectBuff_UnitStatus(_unit_status))
            {
                buff_value += Buff.Collect(_situation_type, Owner, e);
            }


            var weapon_item  = Weapon.ItemObject;
            if (weapon_item != null)
            {
                Item.CollectAttributeValue(weapon_item.Kind, EnumItemAttribute.UnitStatusBuff, (int)_unit_status);                
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




        /// <summary> 물리 위력. </summary>
        public int Calc_Might_Physic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Physics);
            if (weapon_might <= 0)
            {
                // 물리 위력이 0이면 0 리턴.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Strength);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Physics);
           
            return status_unit + status_weapon;
        }

        /// <summary> 마법 위력. </summary>
        public int Calc_Might_Magic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Magic);
            if (weapon_might <= 0)
            {
                // 마법 위력이 0이면 0 리턴.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Magic);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Magic);

            return status_unit + status_weapon;
        }

        public float Calc_Penetration_Defense()
        {
            var item_object = Weapon.ItemObject;
            if (item_object != null)
            {
                // 단검은 방어력을 무시.
                if (item_object.WeaponCategory == EnumWeaponCategory.Dagger)
                    return 1f;
            }
            
            return 0f;
        }

        public float Calc_Penetration_Resistance()
        {
            return 0f;
        }

        /// <summary> 명중. </summary>
        public int Calc_Hit()
        {
            // var unit_luck  = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var unit_skill = GetBuffedUnitStatus(EnumUnitStatus.Skill);            
            var unit_hit   = unit_skill * 3 ;//+ unit_luck / 2;
            var weapon_hit = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Hit);

            return unit_hit + weapon_hit;
        }

        /// <summary> 필살. </summary>
        public int Calc_Critical()
        {
            // var unit_skill      = GetBuffedUnitStatus(EnumUnitStatus.Skill);
            // var unit_critical   = unit_skill / 2;
            // return unit_critical + weapon_critical;
            var    weapon_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Critical);
            return weapon_critical;
        }


        /// <summary> 회피. </summary>
        public int Calc_Dodge()
        {
            // var unit_luck    = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var battle_speed = Calc_Speed();
            var weapon_dodge = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge);
            

            return battle_speed * 2 + weapon_dodge; //+ unit_luck / 2
        }

        /// <summary> 필살 회피. </summary>
        public int Calc_DodgeCritical()
        {
            var unit_luck             = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        /// <summary> 속도. </summary>
        public int Calc_Speed()
        {
            // var unit_weight  = GetBuffedUnitStatus(EnumUnitStatus.Weight);
            var use_magic    = (Weapon != null && Weapon.GetStatus(EnumWeaponStatus.Might_Magic) > 0);

            var unit_power   = (use_magic) ? GetBuffedUnitStatus(EnumUnitStatus.Magic) : GetBuffedUnitStatus(EnumUnitStatus.Strength);
            var unit_speed   = GetBuffedUnitStatus(EnumUnitStatus.Speed);
            var wepon_weight = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - (unit_power + 1) / 2);
        }

        /// <summary> 방어. </summary>
        public int Calc_Defense()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Defense);
        }

        /// <summary> 마방. </summary>
        public int Calc_Resistance()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Resistance);
        }

        public StatusManager_IO Save()
        {
            return new StatusManager_IO()
            {
                UnitStatus  = Status.Save(),
                BuffManager = Buff.Save(),
                Weapon      = Weapon.Save()
            };
        }

        public void Load(StatusManager_IO _snapshot)
        {
            Status.Load(_snapshot.UnitStatus);
            Buff.Load(_snapshot.BuffManager);
            Weapon.Load(_snapshot.Weapon);
        }
    }


    public class StatusManager_IO
    {
        public UnitStatus_IO  UnitStatus  { get; set; } = new();
        public BuffManager_IO BuffManager { get; set; } = new();
        public Weapon_IO      Weapon      { get; set; } = new();

    }

}

/*
// 요거에서 베스타리아 사가 공식으로 일부 수정함. (속도 등.)
// 근데 if 두 개 키우는 능력치 공식 잡았는데... 밸런스는 모르겠네...
// * 공격[전투]     = 힘[공격] + 무기[공격력] + 숙련*(1[1] or 2[2] or 3[3]) + 기타[A](스킬 버프효과 등)
// * 명중           = 기술*2 + 행운/2 + 무기[명중] + 기타[A](스킬 버프효과 등)
// * 필살           = 기술/2 + 무기[필살] + 기타[A]
// * 실제력         = (공격[전투] - 방어[전투])*(1 or 3[7])
// * 속도[8]        = 속도 - (무게 - 체력)[9]
// * 회피           = 속도*2 + 행운/2 + 무기 회피 + 기타[A]
// * 필살 회피      = 행운 + 무기 필살 회피 + 기타[A]
// * 명중력         = 명중 - 회피
// * 필살 발생 확률  = 필살 - 필살 회피
 */

