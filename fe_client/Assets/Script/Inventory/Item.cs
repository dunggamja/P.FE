using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;

public partial class Item 
{
    private Int64 m_id        = 0;
    private Int32 m_kind      = 0;
    private Int32 m_count_cur = 0;
    private Int32 m_count_max = 0;

    public Item(Int64 _id, Int32 _kind)
    {
        m_id   = _id;
        m_kind = _kind;
    }


    public long         ID           => m_id;
    public int          Kind         => m_kind;
    public EnumItemType ItemType     => EnumItemType.Weapon;    
    public int          CurCount     => m_count_cur;             
    public int          MaxCount     => m_count_max;       
    public bool         IsDisposable => true;

    public bool IsEnableAction(IOwner owner, EnumItemActionType _action)
    {
        //var is_system_action = (owner == null);

        switch(_action)
        {
            case EnumItemActionType.Equip:  return IsEnableAction_Weapon_Equip(owner);
            case EnumItemActionType.Unequip:return IsEnableAction_Weapon_Unequip(owner);            
        }

        return false;
    }

    public bool ProcessAction(IOwner owner, EnumItemActionType _action)
    {
        if (!IsEnableAction(owner, _action))
            return false;

        switch (_action)
        {
            case EnumItemActionType.Equip:   return ProcessAction_Weapon_Equip(owner);
            case EnumItemActionType.Unequip: return ProcessAction_Weapon_Unequip(owner);
        }

        return false;
    }


    public LocalizeKey GetLocalizeKey()
    {
        return LocalizeKey.Create(string.Empty, string.Empty);
    }


    
}