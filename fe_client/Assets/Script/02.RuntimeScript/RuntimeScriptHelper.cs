using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Lua.Standard;
using Lua.Unity;
using Battle;
using Cysharp.Threading.Tasks;
using System.Linq.Expressions;



public static class RuntimeScriptHelper
{

    public static T GetLuaValue<T>(this LuaTable _table, string _key)
    {
       if (_table == null)
         return default;

       if (_table.TryGetValue(_key, out var value))
         return value.TryRead<T>(out var result) ? result : default;

       return default;
    }

    public static void SetLuaValue(this LuaTable _table, string _key, int _value)         => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, Int64 _value)       => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, string _value)      => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, LuaFunction _value) => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, LuaTable _value)    => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, bool _value)        => _table[_key] = _value;

   

    public static void FromLua(LuaTable _table, out TAG_INFO _o)
    {
        if (_table == null)
        {
        _o = TAG_INFO.Create(EnumTagType.None, 0);
        return;
        }

        var tag_type  = _table.GetLuaValue<int>("TagType");
        var tag_value = _table.GetLuaValue<Int64>("TagValue");        
        _o            = TAG_INFO.Create((EnumTagType)tag_type, tag_value);
    }

    public static LuaTable ToLua(this TAG_INFO _tag_info)
    {
        var table = new LuaTable();
        table.SetLuaValue("TagType", (int)_tag_info.TagType);
        table.SetLuaValue("TagValue", (Int64)_tag_info.TagValue);
        return table;
    }

    public static void FromLua(LuaTable _table, out TAG_DATA _o)
    {
        if (_table == null)
        { 
        _o = TAG_DATA.Create(TAG_INFO.Create(EnumTagType.None, 0), EnumTagAttributeType.None, TAG_INFO.Create(EnumTagType.None, 0));
        return;
        }

        var lua_tag    = _table.GetLuaValue<LuaTable>("TagInfo");
        var lua_attr   = _table.GetLuaValue<int>("Attribute");
        var lua_target = _table.GetLuaValue<LuaTable>("TargetInfo");


        FromLua(lua_tag, out TAG_INFO tag_info);
        FromLua(lua_target, out TAG_INFO target_info);
        _o = TAG_DATA.Create(tag_info, (EnumTagAttributeType)lua_attr, target_info);        
    }

    public static LuaTable ToLua(this TAG_DATA _tag_data)
    {
        var table = new LuaTable();
        table.SetLuaValue("TagInfo", _tag_data.TagInfo.ToLua());
        table.SetLuaValue("Attribute", (int)_tag_data.Attribute);
        table.SetLuaValue("TargetInfo", _tag_data.TargetInfo.ToLua());
        return table;
    } 

    public static void FromLua(LuaTable _table, out DIALOGUE_PORTRAIT _o)
    {
        if (_table == null)
        {
            _o = default(DIALOGUE_PORTRAIT);
            return;
        }

        var name            = _table.GetLuaValue<string>("Name");
        var portrait_asset  = _table.GetLuaValue<string>("PortraitAsset");
        var portrait_sprite = _table.GetLuaValue<string>("PortraitSprite");

        _o = new DIALOGUE_PORTRAIT()
        {
            Name           = name,
            PortraitAsset  = portrait_asset,
            PortraitSprite = portrait_sprite
        };        
    }

    // 다이얼로그를 Lua로 전달할 일은 없을듯?
    // public static LuaTable ToLua(this DIALOGUE_PORTRAIT _dialogue_portrait)
    // {
    //     var table = new LuaTable();
    //     table.SetLuaValue("Name", _dialogue_portrait.Name);
    //     table.SetLuaValue("PortraitAsset", _dialogue_portrait.PortraitAsset);
    //     table.SetLuaValue("PortraitSprite", _dialogue_portrait.PortraitSprite);
    //     return table;
    // }

    public static void FromLua(LuaTable _table, out DIALOGUE_DATA _o)
    {
        if (_table == null)
        {
        _o = default(DIALOGUE_DATA);
        return;
        }
        
        var is_active = _table.GetLuaValue<bool>("IsActive");
        var position  = _table.GetLuaValue<int>("Position");
        var dialogue  = _table.GetLuaValue<string>("Dialogue");
        FromLua(_table.GetLuaValue<LuaTable>("Portrait"), out DIALOGUE_PORTRAIT portrait);

        _o = new DIALOGUE_DATA()
        {
            IsActive = is_active,
            Position = (DIALOGUE_DATA.EnumPosition)position,
            Portrait = portrait,
            Dialogue = dialogue
        };
    }
    
    // 다이얼로그를 Lua로 전달할 일은 없을듯?
    // public static LuaTable ToLua(this DIALOGUE_DATA _dialogue_data)
    // {
    //     var table = new LuaTable();
    //     table.SetLuaValue("IsActive", _dialogue_data.IsActive);
    //     table.SetLuaValue("Position", (int)_dialogue_data.Position);
    //     table.SetLuaValue("Dialogue", _dialogue_data.Dialogue);
    //     table.SetLuaValue("Portrait", _dialogue_data.Portrait.ToLua());
    //     return table;
    // }


    public static void FromLua(LuaTable _table, out DIALOGUE_SEQUENCE _o)
    {
        if (_table == null)
        {
        _o = default(DIALOGUE_SEQUENCE);
        return;
        }
        
        var close_dialogue = _table.GetLuaValue<bool>("CloseDialogue");
        var dialogue_data  = _table.GetLuaValue<LuaTable>("DialogueData");


        Queue<DIALOGUE_DATA> dialogue_data_queue = new();

        if (dialogue_data != null)
        {
            foreach( var e in dialogue_data.GetArraySpan())
            {
                if (e.TryRead<LuaTable>(out var table) == false)
                    continue;
                
                FromLua(table, out DIALOGUE_DATA data);
                dialogue_data_queue.Enqueue(data);
            }
        }



        _o = new DIALOGUE_SEQUENCE()
        {
            ID            = Util.GenerateID(),
            CloseDialogue = close_dialogue,
            DialogueData  = dialogue_data_queue
        };
    }

    // 다이얼로그를 Lua로 전달할 일은 없을듯?
    // public static LuaTable ToLua(this DIALOGUE_SEQUENCE _dialogue_sequence)
    // {
    //     var table = new LuaTable();
    //     table.SetLuaValue("ID", _dialogue_sequence.ID);
    //     table.SetLuaValue("CloseDialogue", _dialogue_sequence.CloseDialogue);

    //     var dialogue_data = new LuaTable();
    //     int index = 1;
    //     foreach (var e in _dialogue_sequence.DialogueData)
    //     {
    //         dialogue_data[index++] = (e.ToLua());
    //     }

    //     table.SetLuaValue("DialogueData", dialogue_data);
    //     return table;
    // }


}