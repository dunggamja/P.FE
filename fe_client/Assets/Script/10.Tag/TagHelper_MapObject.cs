using System;
using System.Collections;
using System.Collections.Generic;
using Lua;
using UnityEngine;
using Battle;

public static partial class TagHelper
{
    static bool Verify_TagType_MapObject(EnumTagType _tag_type)
    {
        switch(_tag_type)
        {
        case EnumTagType.MapObject:
            return true;
        }
        return false;
    }

    public static void Collect_MapObject(this TAG_INFO _tag_info, List<MapObject> _list)
    {
        if (Verify_TagType_MapObject(_tag_info.TagType))
        {
            if (_tag_info.TagType == EnumTagType.MapObject)
            {
                var map_object = MapObjectManager.Instance.SeekMapObject(_tag_info.TagValue);
                if (map_object != null)
                    _list.Add(map_object);
            }            
        }
    }


}
