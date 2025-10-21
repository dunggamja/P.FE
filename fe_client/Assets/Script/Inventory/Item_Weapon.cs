using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Item 
{

    public int  GetWeaponStatus(EnumWeaponStatus _status_type)
    {
        return m_status.GetValue((int)_status_type);
    }

    public bool HasWeaponAttribute(EnumWeaponAttribute _attribute_type)
    {
        return m_attribute.HasValue((int)_attribute_type);
    }

    public void SetWeaponStatus(EnumWeaponStatus _status_type, int _value)
    {
        m_status.SetValue((int)_status_type, _value);
    }

    public void SetWeaponAttribute(EnumWeaponAttribute _attribute_type, bool _value)
    {
        m_attribute.SetValue((int)_attribute_type, _value);
    }   


    bool IsEnableAction_Weapon_Equip(IOwner owner)
    {
        if (owner == null)
            return false;

        // �����ڸ� ã�� �� ����.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;
        // ���Ⱑ �ƴ� �� �� ����.
        if (ItemType != EnumItemType.Weapon)                
            return false;
        
        // ������ ������ ����.
        var owner_weapon = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon == null || owner_weapon.ItemID == ID)
            return false;
        
        // todo: owner ���� ���� ���� üũ
        return true;
    }

    bool IsEnableAction_Weapon_Unequip(IOwner owner)
    {
        if (owner == null)
            return false;

        // �����ڸ� ã�� �� ����.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;

        // �ٸ� ������ ����.
        var owner_weapon = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon == null || owner_weapon.ItemID != ID)
            return false;

        // todo: �ͼ� ���� üũ.?
            
        return true;
    }

    bool ProcessAction_Weapon_Equip(IOwner owner)
    {
        // �����ڸ� ã�� �� ����.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;

        // ������ ����.
        var owner_weapon  = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon != null)                                    
            owner_weapon.Equip(ID);

        return true;
    }

    bool ProcessAction_Weapon_Unequip(IOwner owner)
    {
        // �����ڸ� ã�� �� ����.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;

        // ������ ����.
        var owner_weapon  = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon != null)                                    
            owner_weapon.Unequip();
            
        return true;
    }


}
