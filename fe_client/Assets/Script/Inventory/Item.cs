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

    private Item(Int64 _id, Int32 _kind)
    {
        m_id   = _id;
        m_kind = _kind;
    }

    public Item()
    { }


    public  long          ID           => m_id;
    public  int           Kind         => m_kind;
    public  EnumItemType  ItemType     => EnumItemType.Weapon;    
    public  int           CurCount     => m_count_cur;             
    public  int           MaxCount     => m_count_max;       
    public  bool          IsDisposable => true;

    private BaseContainer m_status    = new BaseContainer();
    private BaseContainer m_attribute = new BaseContainer();

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


    public LocalizeKey GetLocalizeName()
    {
        var (table, key) = DataManager.Instance.WeaponSheet.GetLocalizeName(Kind);
        return LocalizeKey.Create(table, key);
    }

    public LocalizeKey GetLocalizeDesc()
    {
        var (table, key) = DataManager.Instance.WeaponSheet.GetLocalizeDesc(Kind);
        return LocalizeKey.Create(table, key);
    }

    public static Item Create(Int64 _id, Int32 _kind)
    {
        var item      = new Item(_id, _kind);

        var status    = DataManager.Instance.WeaponSheet.GetStatus(item.Kind);
        var attribute = DataManager.Instance.WeaponSheet.GetAttribute(item.Kind);

        // 무기 스탯 설정
        if (status != null)
        {
            item.SetWeaponStatus(EnumWeaponStatus.Might_Physics,  status.PHYSICS);
            item.SetWeaponStatus(EnumWeaponStatus.Might_Magic,    status.MAGIC);
            item.SetWeaponStatus(EnumWeaponStatus.Hit,            status.HIT);
            item.SetWeaponStatus(EnumWeaponStatus.Critical,       status.CRITICAL);
            item.SetWeaponStatus(EnumWeaponStatus.Weight,         status.WEIGHT);
            item.SetWeaponStatus(EnumWeaponStatus.Dodge,          status.DODGE);
            item.SetWeaponStatus(EnumWeaponStatus.Dodge_Critical, status.DODGE_CRITICAL);
            item.SetWeaponStatus(EnumWeaponStatus.Range,          status.RANGE);
            item.SetWeaponStatus(EnumWeaponStatus.Range_Min,      status.RANGE_MIN);
        }

        // 무기 속성 설정.
        if (attribute != null)
        {
            foreach (var e in attribute)
            {
                item.SetWeaponAttribute((EnumWeaponAttribute)e.ATTRIBUTE, true);
            }
        }

        return item;
    }

    public Item_IO Save()
    {
        return new Item_IO()
        {
            ID        = ID,
            Kind      = Kind,
            Count     = CurCount,
            MaxCount  = MaxCount,
            Status    = m_status.Save(),
            Attribute = m_attribute.Save()
        };
    }

    public void Load(Item_IO _snapshot)
    {
        m_id        = _snapshot.ID;
        m_kind      = _snapshot.Kind;
        m_count_cur = _snapshot.Count;
        m_count_max = _snapshot.MaxCount;
        m_status.Load(_snapshot.Status);
        m_attribute.Load(_snapshot.Attribute);
    }
}


public class Item_IO
{
    public Int64        ID        { get; set; } = 0;
    public Int32        Kind      { get; set; } = 0;
    public Int32        Count     { get; set; } = 0;
    public Int32        MaxCount  { get; set; } = 0;

    public BaseContainer_IO Status    { get; set; } = new();
    public BaseContainer_IO Attribute { get; set; } = new();

}

