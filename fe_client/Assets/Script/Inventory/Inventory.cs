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

    public void CollectItem(ref List<Item> _list_result, Func<Item, bool> _func_condition)
    {
        if (_list_result == null)
            return;

        
        foreach(var e in m_repository_list)
        {
            if (_func_condition == null || _func_condition(e))
                _list_result.Add(e);
        }
    }

    public void CollectItemByType(ref List<Item> _list_result, EnumItemType _item_type)
    {
        CollectItem(ref _list_result, (e) => e.ItemType == _item_type);
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

        if (!item.ProcessAction(owner_entity, _action_type))
            return false;

        // TODO: 아이템 감소.

        return true;
    }

    public Inventory_IO Save()
    {
        var items = new List<Item_IO>();

        // 아이템 스냅샷 생성.
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