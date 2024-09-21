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

    public List<Item> CollectItem(Func<Item, bool> _func_condition)
    {
        var list_result = new List<Item>(10);

        if (_func_condition != null)
        {
            foreach(var e in m_repository_list)
            {
                if (_func_condition(e))
                    list_result.Add(e);
            }
        }

        return list_result;
    }

    public List<Item> CollectItemByType(EnumItemType _item_type)
    {
        return CollectItem((e) => e.ItemType == _item_type);
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

}