using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;

// 공용 인벤토리
public class GlobalInventoryManager : Singleton<GlobalInventoryManager>
{
    private Inventory                               m_inventory      = new(); // 공용 인벤토리 (보관소)
    private Dictionary<EnumResourceCategory, Int64> m_resource_point = new(); // 자원 포인트 (금화 등)

    public Inventory Inventory => m_inventory;


    protected override void Init()
    {
        base.Init();

        m_inventory.SetOwner(null); // 공용 인벤토리의 소유자는 없음.
    }

    public Int64 GetResource(EnumResourceCategory _category)
    {
        return m_resource_point.TryGetValue(_category, out var value) ? value : 0;
    }
    public void SetResource(EnumResourceCategory _category, Int64 _amount)
    {
        // 0 미만은 없음.
        _amount = Math.Max(0, _amount);

        m_resource_point[_category] = _amount;
    }

    public void IncreaseResource(EnumResourceCategory _category, Int64 _amount)
    {        
        SetResource(_category, GetResource(_category) + _amount);
    }

    public void DecreaseResource(EnumResourceCategory _category, Int64 _amount)
    {
        SetResource(_category, GetResource(_category) - _amount);
    }
}