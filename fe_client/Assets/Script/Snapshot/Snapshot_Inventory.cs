using System;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ItemSnapshot
{
    public Int64             ID        { get; set; } = 0;
    public Int32             Kind      { get; set; } = 0;
    public Int32             Count     { get; set; } = 0;
    public Int32             MaxCount  { get; set; } = 0;

    public ContainerSnapshot Status    { get; set; } = new();
    public ContainerSnapshot Attribute { get; set; } = new();

    public static ItemSnapshot Create(
        Int64             _id,
        Int32             _kind,
        Int32             _count,
        Int32             _max_count,
        ContainerSnapshot _status,
        ContainerSnapshot _attribute)
    {
        var snapshot       = new ItemSnapshot();
        snapshot.ID        = _id;
        snapshot.Kind      = _kind;
        snapshot.Count     = _count;
        snapshot.MaxCount  = _max_count;
        snapshot.Status    = _status;
        snapshot.Attribute = _attribute;
        return snapshot;
    }
}


public class InventorySnapshot
{
    public List<ItemSnapshot> Items { get; set; } = new();

    public static InventorySnapshot Create(
        IEnumerable<ItemSnapshot> _items)
    {
        var snapshot   = new InventorySnapshot();
        snapshot.Items = new List<ItemSnapshot>(_items);
        return snapshot;
    }
}