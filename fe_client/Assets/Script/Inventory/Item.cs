using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;
using Shapes;
using R3;

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

    private Item(EnumItemType _item_type, Int64 _id, Int32 _kind, Int32 _count_cur)
    {
        m_id        = _id;
        m_kind      = _kind;
        m_item_type = _item_type;
        m_count_cur = _count_cur;
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


    public EnumWeaponCategory WeaponCategory 
    {
        get
        {
            if (ItemType != EnumItemType.Weapon)
                return EnumWeaponCategory.None;
            
            return (EnumWeaponCategory)ItemCategory;
        }
    }

    public int MaxCount
    {
        get
        {
            var sheet_item = DataManager.Instance.ItemSheet.GetStatus(Kind);
            if (sheet_item == null)
                return 0;

            return sheet_item.MAX_COUNT;
        }
    }



    public bool DecreaseCount()
    {
        if (CurCount <= 0)
            return false;

        --m_count_cur;
        return true;
    }

    static public Observable<string> GetNameText(Item _item, bool _show_count = false)
    {
        if (_item == null)
            return Observable.Empty<string>();

        var localize_key = _item.GetLocalizeName();
        var text_subject = LocalizationManager.Instance.GetTextObservable(localize_key.Table, localize_key.Key);

        if (_show_count)
            text_subject = text_subject.Select(text => $"{text} ({_item.CurCount}/{_item.MaxCount})");

        return text_subject;
    }




    static public Item Create(Int64 _id, Int32 _kind)
    {
        var sheet_item = DataManager.Instance.ItemSheet.GetStatus(_kind);
        var item_type  = (sheet_item != null) ? (EnumItemType)sheet_item.TYPE : EnumItemType.None;
        var item_count = (sheet_item != null) ? sheet_item.MAX_COUNT : 0;
        var item       = new Item(item_type, _id, _kind, item_count);

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
        };
    }

    public void Load(Item_IO _snapshot)
    {
        m_id        = _snapshot.ID;
        m_kind      = _snapshot.Kind;
        m_count_cur = _snapshot.Count;
        m_item_type = (EnumItemType)_snapshot.ItemType;
    }
}

public class Item_IO
{
    public Int64        ID        { get; set; } = 0;
    public Int32        Kind      { get; set; } = 0;
    public Int32        Count     { get; set; } = 0;
    public Int32        ItemType  { get; set; } = 0;
    // public Int32        MaxCount  { get; set; } = 0;

    // public BaseContainer_IO Status    { get; set; } = new();
    // public BaseContainer_IO Attribute { get; set; } = new();
}
