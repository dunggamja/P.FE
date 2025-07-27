using System;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class Item_IO
{
    public Int64        ID        { get; set; } = 0;
    public Int32        Kind      { get; set; } = 0;
    public Int32        Count     { get; set; } = 0;
    public Int32        MaxCount  { get; set; } = 0;

    public BaseContainer_IO Status    { get; set; } = new();
    public BaseContainer_IO Attribute { get; set; } = new();

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