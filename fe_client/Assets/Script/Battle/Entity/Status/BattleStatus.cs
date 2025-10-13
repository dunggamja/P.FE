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




        /// <summary> ���ݷ�. </summary>
        public int Calc_Might_Physic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Physics);
            if (weapon_might <= 0)
            {
                // ���Ⱑ ������ �������� ����.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Strength);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Physics);
           
            return status_unit + status_weapon;
        }

        /// <summary> �������ݷ�. </summary>
        public int Calc_Might_Magic()
        {
            var weapon_might = Weapon.GetStatus(EnumWeaponStatus.Might_Magic);
            if (weapon_might <= 0)
            {
                // ���Ⱑ ������ �������� ����.
                return 0;
            }

            var status_unit   = GetBuffedUnitStatus(EnumUnitStatus.Magic);
            var status_weapon = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Might_Magic);

            return status_unit + status_weapon;
        }

        /// <summary> ���� </summary>
        public int Calc_Hit()
        {
            var unit_skill = GetBuffedUnitStatus(EnumUnitStatus.Skill);
            var unit_luck  = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            
            var unit_hit   = unit_skill * 2 + unit_luck / 2;
            var weapon_hit = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Hit);

            return unit_hit + weapon_hit;
        }

        /// <summary> ġ�� </summary>
        public int Calc_Critical()
        {
            var unit_skill      = GetBuffedUnitStatus(EnumUnitStatus.Skill);

            var unit_critical   = unit_skill / 2;
            var weapon_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Critical);

            return unit_critical + weapon_critical;
        }


        /// <summary> ȸ�� </summary>
        public int Calc_Dodge()
        {
            var battle_speed = Calc_Speed();
            var unit_luck    = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge);

            return battle_speed * 2 + unit_luck / 2 + weapon_dodge;
        }

        /// <summary> ġ�� ȸ�� </summary>
        public int Calc_DodgeCritical()
        {
            var unit_luck             = GetBuffedUnitStatus(EnumUnitStatus.Luck);
            var weapon_dodge_critical = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Dodge_Critical);

            return unit_luck + weapon_dodge_critical;
        }

        /// <summary> �ӵ� </summary>
        public int Calc_Speed()
        {
            var unit_speed   = GetBuffedUnitStatus(EnumUnitStatus.Speed);
            var unit_weight  = GetBuffedUnitStatus(EnumUnitStatus.Weight);
            var wepon_weight = GetBuffedWeaponStatus(Weapon, EnumWeaponStatus.Weight);

            return unit_speed - Math.Max(0, wepon_weight - unit_weight);
        }

        /// <summary> ���� </summary>
        public int Calc_Defense()
        {
            return GetBuffedUnitStatus(EnumUnitStatus.Defense);
        }

        /// <summary> �������� </summary>
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
 �Ŀ� if ������Ű���� �ɷ�ġ ���� �ܾ�µ�... ���ڵ��� �������ȳ�...
 * ����[����]     = ��[����] + ����[������] ����*(1[1] or 2[2] or 3[3]) + ��Ÿ[A](��ų ����ȿ�� ��)
 * ����           = ���?*2 + ���?/2 + ����[������] ���� + ��Ÿ[A](��ų ����ȿ�� ��)
 * �ʻ�           = ���?/2 + ����[������] �ʻ� + ��Ÿ[A]
 * ���ط�         = (����[����] - ����[����])*(1 or 3[7])
 * ����[8]        = �ӵ� - (���� - ü��)[9]
 * ȸ��           = ����*2 + ���?/2 + ���� ȸ�� + ��Ÿ[A]
 * �ʻ� ȸ��      = ���? + ���� �ʻ� ȸ�� + ��Ÿ[A]
 * ���߷�         = ���� - ȸ��
 * �ʻ� �߻� Ȯ��  = �ʻ� - �ʻ� ȸ��
 */

