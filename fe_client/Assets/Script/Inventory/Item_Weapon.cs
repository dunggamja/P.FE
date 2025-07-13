using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Item 
{

    BaseContainer m_weapon_status    = new BaseContainer();
    BaseContainer m_weapon_attribute = new BaseContainer();


    public int  GetWeaponStatus(EnumWeaponStatus _status_type)
    {
        return m_weapon_status.GetValue((int)_status_type);
    }

    public bool HasWeaponAttribute(EnumWeaponAttribute _attribute_type)
    {
        return m_weapon_attribute.HasValue((int)_attribute_type);
    }

    public void SetWeaponStatus(EnumWeaponStatus _status_type, int _value)
    {
        // 테스트용 코드... 실제로 사용할 일은 없을 듯?
        m_weapon_status.SetValue((int)_status_type, _value);
    }

    public void SetWeaponAttribute(EnumWeaponAttribute _attribute_type, bool _value)
    {
        // 테스트용 코드... 실제로 사용할 일은 없을 듯?
        m_weapon_attribute.SetValue((int)_attribute_type, _value);
    }   


    bool IsEnableAction_Weapon_Equip(IOwner owner)
    {
        if (owner == null)
            return false;

        // 소유자를 찾을 수 없음.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;
        // 무기가 아닌 것 을 장착.
        if (ItemType != EnumItemType.Weapon)                
            return false;
        
        // 동일한 아이템 장착.
        var owner_weapon = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon == null || owner_weapon.ItemID == ID)
            return false;
        
        // todo: owner 무기 장착 가능 체크
        return true;
    }

    bool IsEnableAction_Weapon_Unequip(IOwner owner)
    {
        if (owner == null)
            return false;

        // 소유자를 찾을 수 없음.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;

        // 다른 아이템 해제.
        var owner_weapon = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon == null || owner_weapon.ItemID != ID)
            return false;

        // todo: 귀속 제한 체크.?
            
        return true;
    }

    bool ProcessAction_Weapon_Equip(IOwner owner)
    {
        // 소유자를 찾을 수 없음.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;

        // 아이템 장착.
        var owner_weapon  = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon != null)                                    
            owner_weapon.Equip(ID);

        return true;
    }

    bool ProcessAction_Weapon_Unequip(IOwner owner)
    {
        // 소유자를 찾을 수 없음.
        var owner_entity  = EntityManager.Instance.GetEntity(owner.ID);
        if (owner_entity == null)
            return false;

        // 아이템 장착.
        var owner_weapon  = owner_entity.StatusManager.Weapon; //as Weapon;
        if (owner_weapon != null)                                    
            owner_weapon.Unequip();
            
        return true;
    }


}
