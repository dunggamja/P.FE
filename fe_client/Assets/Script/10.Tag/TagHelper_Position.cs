using System;
using System.Collections;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using Battle;

public static partial class TagHelper
{
    const Int64 POSITION_VALUE_MAX = Constants.MAX_MAP_SIZE * Constants.MAX_MAP_SIZE;

    static bool Verify_TagType_Position(EnumTagType _tag_type)
    {
        switch(_tag_type)
        {
        case EnumTagType.Position:
        case EnumTagType.Position_Rect:
            return true;
        }

        return false;
    }

    static (int x, int y) ToPosition(Int64 _value)
    {
        _value %= POSITION_VALUE_MAX;
        return ((int)(_value / Constants.MAX_MAP_SIZE), (int)(_value % Constants.MAX_MAP_SIZE));
    }

    static Int64 PositionToValue(int _x, int _y)
    {
        _x %= Constants.MAX_MAP_SIZE;
        _y %= Constants.MAX_MAP_SIZE;

        return _x * Constants.MAX_MAP_SIZE + _y;      
    }

    public static (int x, int y) ToPosition(this TAG_INFO _tag_info)
    {
        if (_tag_info.TagType == EnumTagType.Position)
        return ToPosition(_tag_info.TagValue);

        return (0, 0);
    }

    public static (int min_x, int min_y, int max_x, int max_y) ToPositionRect(this TAG_INFO _tag_info)
    {
        if (_tag_info.TagType != EnumTagType.Position_Rect)
        return (0, 0, 0, 0);


        var value     = _tag_info.TagValue;         
        var max_value = ToPosition(value);
        var min_value = ToPosition(value / POSITION_VALUE_MAX);

        return (min_value.x, min_value.y, max_value.x, max_value.y);
    }


    public static Int64 PositionRectToValue(int _min_x, int _min_y, int _max_x, int _max_y)
    {
        var value = PositionToValue(_min_x, _min_y);
        value    *= POSITION_VALUE_MAX;
        value    += PositionToValue(_max_x, _max_y);

        return value;
    }

    public static (int x, int y) Peek_Position(this TAG_INFO _tag_info)
    {
        using var list_position = ListPool<(int x, int y)>.AcquireWrapper();    
        Collect_Position(_tag_info, list_position.Value);

        // 가장 처음에 표시되는 위치를 가져와봅시다.
        if (0 < list_position.Value.Count)
            return list_position.Value[0];

        return (0, 0);
    }

    public static void Collect_Position(this TAG_INFO _tag_info, List<(int x, int y)> _list)
    {
        if (Verify_TagType_Position(_tag_info.TagType))
        {
            // 위치 타입에 따라 처리.
            switch(_tag_info.TagType)
            {
            // POINT 
            case EnumTagType.Position:
                {
                    _list.Add(ToPosition(_tag_info));
                }
                break;

            // RECTANGLE
            case EnumTagType.Position_Rect:
                {
                    var (min_x, min_y, max_x, max_y) = ToPositionRect(_tag_info);
                    for(int x = min_x; x <= max_x; x++)
                        for(int y = min_y; y <= max_y; y++)
                            _list.Add((x, y));
                }
                break;
            }
        }
        else if (Verify_TagType_Entity(_tag_info.TagType))
        {
            // 엔티티들 위치 모음.
            using var list_entity = ListPool<Entity>.AcquireWrapper();
            Collect_Entity(_tag_info, list_entity.Value);

            foreach(var e in list_entity.Value)
            {
                _list.Add(e.Cell);
            }            
        }
    }    

}