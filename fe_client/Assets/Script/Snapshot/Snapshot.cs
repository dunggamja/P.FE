using System;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSnapshot
{
    public List<(int key, int value)> Values { get; set; } = new();

    public static ContainerSnapshot Create(IEnumerable<(int key, int value)> _values)
    {
        var snapshot    = new ContainerSnapshot();
        snapshot.Values = new List<(int key, int value)>(_values);
        return snapshot;
    }
}






