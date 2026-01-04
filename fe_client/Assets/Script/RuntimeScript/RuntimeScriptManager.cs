using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Lua.Standard;
using Lua.Unity;
using Battle;
using Cysharp.Threading.Tasks;
using Shapes;

public class RuntimeScriptManager : SingletonMono<RuntimeScriptManager>
{
    private LuaState                     m_lua_state;
    private Dictionary<string, LuaTable> m_lua_tables = new ();

    protected override void OnInitialize()
    {
        base.OnInitialize();

        m_lua_state = LuaState.Create();
        m_lua_state.OpenStandardLibraries();


        // 필요한 함수들은 여기서 모두 등록합시다.
        m_lua_state.Environment["Test"] = new LuaFunction(async (context, ct) =>
        {
            return context.Return();
        });
    }
    
    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);


        if (_is_shutdown)
            return;


        if (m_lua_state != null)
        {
            m_lua_state.Dispose();
            m_lua_state = null;
        }
    }


    private LuaTable GetLuaTable(string _name)
    {
       if (m_lua_tables.TryGetValue(_name, out var table))
         return table;

       return null;
    }

    private LuaTable TryAddLuaTable(string _name)
    {
      var table  = GetLuaTable(_name);
      if (table != null)
        return table;

      table                          = new LuaTable();
      m_lua_state.Environment[_name] = table;
      m_lua_tables[_name]            = table;       

      return table;
    }


    public T GetLuaValue<T>(string _name, string _key)
    {
       var table = GetLuaTable(_name);
       if (table == null)
         return default;

       return table.GetLuaValue<T>(_key);
    }


    public void SetLuaValue(string _name, string _key, int _value)         => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, double _value)      => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, float _value)       => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, bool _value)        => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, string _value)      => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, LuaTable _value)    => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, LuaFunction _value) => TryAddLuaTable(_name).SetLuaValue(_key, _value);     
    
}

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

      // 자주 사용하는 값 타입만 오버로드
    public static void SetLuaValue(this LuaTable _table, string _key, int _value)         => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, double _value)      => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, float _value)       => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, bool _value)        => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, string _value)      => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, LuaTable _value)    => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, LuaFunction _value) => _table[_key] = _value;


}
