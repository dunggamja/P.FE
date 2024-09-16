using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;

public class Item : IItem
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


    public bool IsEnableAction(EnumItemActionAttribute _action)
    {
        return true;
    }

    public bool ProcessAction(EnumItemActionAttribute _action)
    {
        

        switch (_action)
        {
            case EnumItemActionAttribute.Equip:
            break;
        }


        return true;
    }


    
}