using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;


public class Inventory
{
    public Int64 OwnerID { get; private set; }

    Dictionary<Int64, Item> m_repository      = new (10);
    List<Item>              m_repository_list = new (10);

    public int  Count    => m_repository_list.Count;
    public int  MaxCount => 8; // 일단 하드코딩.

    public bool Initialize(IOwner _owner)
    {
        if (_owner == null)
            return false;

        OwnerID = _owner.ID;

        return true;
    }


    public bool AddItem(Item _item)
    {
        if (_item == null)
            return false;

        if (m_repository.ContainsKey(_item.ID))
            return false;

        m_repository.Add(_item.ID, _item);
        m_repository_list.Add(_item);
        return true;
    }

    public Item GetItem(Int64 _id)
    {
        return m_repository.TryGetValue(_id, out var item) ? item : null;
    }

    public bool RemoveItem(Int64 _id)
    {
        if (m_repository.Remove(_id))
        {
            var index = m_repository_list.FindIndex(e => e.ID == _id);
            if (0 <= index && index < m_repository_list.Count)
                m_repository_list.RemoveAt(index);

            return true;
        }

        return false;
    }

    public void CollectItem(List<Item> _list_result, Func<Item, bool> _func_condition = null)
    {
        if (_list_result == null)
            return;

        
        foreach(var e in m_repository_list)
        {
            if (_func_condition == null || _func_condition(e))
                _list_result.Add(e);
        }
    }

    public void CollectItemByType(List<Item> _list_result, EnumItemType _item_type, Func<Item, bool> _func_condition = null)
    {
        CollectItem(_list_result, (e) => e.ItemType == _item_type && (_func_condition == null || _func_condition(e)));
    }


    public void CollectItem_Weapon_Available(List<Item> _list_result, Entity _entity)
    {
        CollectItemByType(
            _list_result, 
            EnumItemType.Weapon,
            e => e.WeaponCategory != EnumWeaponCategory.Wand && _entity.IsEnableAction(e, EnumItemActionType.Equip));
    }

    public void CollectItem_Wand_Available(List<Item> _list_result, Entity _entity)
    {
        CollectItemByType(
            _list_result, 
            EnumItemType.Weapon,
            e => e.WeaponCategory == EnumWeaponCategory.Wand && _entity.IsEnableAction(e, EnumItemActionType.Equip));
    }

    public void ForEachItem(Action<Item> _action)
    {
        foreach(var e in m_repository_list)
        {
            _action(e);
        }
    }



    
    public bool ProcessAction(Int64 _id, EnumItemActionType _action_type)
    {
        var owner_entity  = EntityManager.Instance.GetEntity(OwnerID);
        if (owner_entity == null)
            return false;
        
        var item = GetItem(_id);
        if (item == null)
            return false;

        if (!owner_entity.ProcessAction(item, _action_type))
            return false;

        // TODO: 아이템 버프 갱신.

        return true;
    }

    public int GetItemOrder(Int64 _id)
    {
        var item = GetItem(_id);
        if (item == null)
            return -1;

        return m_repository_list.IndexOf(item);
    }


    public void SetItemOrder(Int64 _id, int _order)
    {
        var item = GetItem(_id);
        if (item == null)
            return;

        // 아이템 정렬순서 변경.

        m_repository_list.Remove(item);

        _order = Math.Clamp(_order, 0, m_repository_list.Count);

        m_repository_list.Insert(_order, item);
    }


    public Inventory_IO Save()
    {
        var items = new List<Item_IO>();

        // 아이템 저장.
        foreach(var e in m_repository_list)
        {
            items.Add(e.Save());
        }

        return new Inventory_IO()
        {
            Items = items
        };  
    }

    public void Load(Inventory_IO _snapshot)
    {
        m_repository.Clear();
        m_repository_list.Clear();

        foreach (var e in _snapshot.Items)
        {
            var item = new Item();            
            item.Load(e);
            AddItem(item);
        }
    }
}


public class Inventory_IO
{
    public List<Item_IO> Items { get; set; } = new();

    public static Inventory_IO Create(
        IEnumerable<Item_IO> _items)
    {
        var snapshot   = new Inventory_IO();
        snapshot.Items = new List<Item_IO>(_items);
        return snapshot;
    }
}