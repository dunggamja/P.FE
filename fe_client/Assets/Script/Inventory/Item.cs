using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;
using Shapes;

[Serializable]
public partial class Item 
{
    [SerializeField]
    private Int64        m_id        = 0;
    [SerializeField]
    private Int32        m_kind      = 0;
    [SerializeField]
    private Int32        m_count_cur = 0;

    [SerializeField]
    private EnumItemType m_item_type = EnumItemType.None;

    // [SerializeField]
    // private Int32 m_count_max = 0;

    private Item(EnumItemType _item_type, Int64 _id, Int32 _kind)
    {
        m_id        = _id;
        m_kind      = _kind;
        m_item_type =  _item_type;
    }

    public Item()
    { }


    public  long          ID           => m_id;
    public  int           Kind         => m_kind;
    public  int           CurCount     => m_count_cur;             
    // public  int           MaxCount     => m_count_max;       
    public  bool          IsDisposable => true;
    public  EnumItemType  ItemType     => m_item_type;   

    public  int           ItemCategory
    {
        get
        {
            var sheet_item = DataManager.Instance.ItemSheet.GetStatus(Kind);
            if (sheet_item == null)
                return 0;

            return sheet_item.CATEGORY;
        }
    }


    // private BaseContainer m_status    = new BaseContainer();
    // private BaseContainer m_attribute = new BaseContainer();


    // public bool IsEnableAction(IOwner owner, EnumItemActionType _action)
    // {
    //     //var is_system_action = (owner == null);

    //     switch(_action)
    //     {
    //         case EnumItemActionType.Equip:  return IsEnableAction_Weapon_Equip(owner);
    //         case EnumItemActionType.Unequip:return IsEnableAction_Weapon_Unequip(owner);            
    //     }

    //     return false;
    // }

    // public bool ProcessAction(IOwner owner, EnumItemActionType _action)
    // {
    //     if (!IsEnableAction(owner, _action))
    //         return false;


    //     var is_success = false;

    //     switch (_action)
    //     {
    //         case EnumItemActionType.Equip:   is_success = ProcessAction_Weapon_Equip(owner);   break;
    //         case EnumItemActionType.Unequip: is_success = ProcessAction_Weapon_Unequip(owner); break;
    //     }

    //     // if (is_success)
    //     // {
    //     //     OnRefreshBuff_Item(Kind, _action);
    //     // }

    //     return false;
    // }




    static public Item Create(Int64 _id, Int32 _kind)
    {
        var sheet_item = DataManager.Instance.ItemSheet.GetStatus(_kind);
        var item_type  = (sheet_item != null) ? (EnumItemType)sheet_item.TYPE : EnumItemType.None;
        var item       = new Item(item_type, _id, _kind);

        return item;
    }


    public Item_IO Save()
    {
        return new Item_IO()
        {
            ID        = ID,
            Kind      = Kind,
            Count     = CurCount,
            ItemType  = (int)ItemType,
            // MaxCount  = MaxCount,
            // Status    = m_status.Save(),
            // Attribute = m_attribute.Save()
        };
    }

    public void Load(Item_IO _snapshot)
    {
        m_id        = _snapshot.ID;
        m_kind      = _snapshot.Kind;
        m_count_cur = _snapshot.Count;
        m_item_type = (EnumItemType)_snapshot.ItemType;
        // m_count_max = _snapshot.MaxCount;
        // m_status.Load(_snapshot.Status);
        // m_attribute.Load(_snapshot.Attribute);
    }



}


public class Item_IO
{
    public Int64        ID        { get; set; } = 0;
    public Int32        Kind      { get; set; } = 0;
    public Int32        Count     { get; set; } = 0;
    public Int32        ItemType  { get; set; } = 0;
    // public Int32        MaxCount  { get; set; } = 0;

    public BaseContainer_IO Status    { get; set; } = new();
    public BaseContainer_IO Attribute { get; set; } = new();

}
