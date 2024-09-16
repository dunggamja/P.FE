using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;


public class Inventory
{
    public Int64 OwnerID { get; private set; }

    Dictionary<Int64, IItem> m_repository = new (10);


    public bool AddItem(IItem _item)
    {
        if (_item == null)
            return false;

        if (m_repository.ContainsKey(_item.ID))
            return false;

        m_repository.Add(_item.ID, _item);
        return true;
    }

    public IItem GetItem(Int64 _id)
    {
        return m_repository.TryGetValue(_id, out var item) ? item : null;
    }

    public bool RemoveItem(Int64 _id)
    {
        return m_repository.Remove(_id);
    }

    
    public bool UseItem(Int64 _id)
    {
        var owner_entity  = EntityManager.Instance.GetEntity(OwnerID);
        if (owner_entity == null)
            return false;
        
        return true;
    }

}