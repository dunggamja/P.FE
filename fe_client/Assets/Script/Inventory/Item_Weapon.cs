using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Item 
{
    

    public int  GetWeaponStatus(EnumWeaponStatus _status_type)
    {
        var status = DataManager.Instance.ItemSheet.GetStatus(Kind);
        if (status == null)
            return 0;

        switch(_status_type)
        {
            case EnumWeaponStatus.Might_Physics:  return status.PHYSICS;
            case EnumWeaponStatus.Might_Magic:    return status.MAGIC;
            case EnumWeaponStatus.Hit:            return status.HIT;
            case EnumWeaponStatus.Critical:       return status.CRITICAL;
            case EnumWeaponStatus.Weight:         return status.WEIGHT;
            case EnumWeaponStatus.Dodge:          return status.DODGE;
            case EnumWeaponStatus.Dodge_Critical: return status.DODGE_CRITICAL;
            case EnumWeaponStatus.Range:          return status.RANGE;
            case EnumWeaponStatus.Range_Min:      return status.RANGE_MIN;
            case EnumWeaponStatus.Proficiency:    return status.PROFICIENCY;
            case EnumWeaponStatus.MaxCount:       return status.MAX_COUNT;
            case EnumWeaponStatus.Price:          return status.PRICE;
        }

        return 0;
    }

    
    public bool HasAttribute(EnumItemAttribute _attribute_type, int _target = 0)
    {
        return GetAttribute(_attribute_type, _target) != 0;
    }



    public int GetAttribute(EnumItemAttribute _attribute_type, int _target = 0)
    {
        var attributes = DataManager.Instance.ItemSheet.GetAttribute(Kind);
        if (attributes == null)
            return 0;


        int total_value = 0;

        foreach (var attribute in attributes)
        {
            if (attribute.TYPE != (int)_attribute_type)
                continue;
            
            if (attribute.TARGET != _target && 0 < _target)
                continue;

            total_value += attribute.VALUE;
        }
            
        return total_value;
    }

    static public void CollectBuffID(Int32 _item_kind, List<Int64> _list_buff_id)
    {
        if (_list_buff_id == null)
            return;

        using var list_collect = ListPool<(int target, int value)>.AcquireWrapper();

        Item.CollectAttribute(_item_kind, EnumItemAttribute.BuffBonus, list_collect.Value);

        foreach (var (_, buff_id) in list_collect.Value)
            _list_buff_id.Add(buff_id);
    }


    static public void CollectAttribute(Int32 _item_kind, EnumItemAttribute _attribute_type, List<(int target, int value)> _list_collect)
    {
        if (_list_collect == null)
            return;


        var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
        if (attributes == null)
            return;

        foreach (var attribute in attributes)
        {
            if (attribute.TYPE != (int)_attribute_type)
                continue;
            
            if (attribute.VALUE == 0)
                continue;

            _list_collect.Add((attribute.TARGET, attribute.VALUE));
        }
    }

    static public int CollectAttributeValue(Int32 _item_kind, EnumItemAttribute _type, int _target)
    {
        var attributes = DataManager.Instance.ItemSheet.GetAttribute(_item_kind);
        if (attributes == null)
            return 0;

        int total_value = 0;

        foreach (var attribute in attributes)
        {
            if (attribute.TYPE != (int)_type)
                continue;

            if (attribute.TARGET != _target && 0 < _target)
                continue;

            total_value += attribute.VALUE;
        }

        return total_value;
    }




}
