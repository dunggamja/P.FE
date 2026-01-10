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
using System.Threading.Tasks;
#pragma warning disable CS1998 

public class RuntimeScriptManager : SingletonMono<RuntimeScriptManager>
{
    private LuaState                     m_lua_state;

    // 변수는 Lua에 저장하지 않는다.
    // 상수 및 콜백만 Lua에 저장한다.
    private Dictionary<string, LuaTable> m_lua_tables = new ();

    protected override void OnInitialize()
    {
        base.OnInitialize();

        m_lua_state = LuaState.Create();
        m_lua_state.OpenStandardLibraries();

        // Enum 값 등록.
        RegisterEnum<EnumUnitAttribute>();
        RegisterEnum<EnumUnitStatus>();
        RegisterEnum<EnumWeaponCategory>();
        RegisterEnum<EnumItemConsumeCategory>();
        RegisterEnum<EnumResourceCategory>();
        RegisterEnum<EnumItemAttribute>();
        RegisterEnum<EnumWeaponStatus>();
        RegisterEnum<EnumBuffStatus>();
        RegisterEnum<EnumBuffTarget>();
        RegisterEnum<EnumBuffContentsType>();
        RegisterEnum<EnumUnitCommandType>();
        RegisterEnum<EnumTagType>();
        RegisterEnum<EnumTagAttributeType>();
        RegisterEnum<EnumScenarioTrigger>();
        RegisterEnum<EnumScenarioCondition>();

        // constants
        SetLuaValue("Constants", "MAX_MAP_SIZE", Constants.MAX_MAP_SIZE);


        // 필요한 함수들 등록.
        RegisterFunction();

        // // 기본 스크립트 로드.
        // Load_Default_Script().GetAwaiter().GetResult();
    }
    
    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);


        // if (_is_shutdown)
        //     return;


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
    public void SetLuaValue(string _name, string _key, Int64 _value)       => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    public void SetLuaValue(string _name, string _key, LuaFunction _value) => TryAddLuaTable(_name).SetLuaValue(_key, _value);     
    public void SetLuaValue(string _name, string _key, LuaTable _value)    => TryAddLuaTable(_name).SetLuaValue(_key, _value);
    

    void RegisterEnum<T>() where T : Enum
    {
        var table_name = typeof(T).Name;
        var table      = TryAddLuaTable(table_name);
        foreach (var value in Enum.GetValues(typeof(T)))
        {
            table.SetLuaValue(value.ToString(), (int)value);
        }
    }


    void RegisterFunction()
    {        
        // TagManager
        SetLuaValue("TagManager", "SetTag", new LuaFunction(async (context, ct) =>
        {
            TagHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_DATA tag_data);
            TagManager.Instance.SetTag(tag_data);
            return context.Return();
        }));
    }

    async UniTask Load_Default_Script()
    {
        await Load_Script("runtimescript/common");
        await Load_Script("runtimescript/tag");        
    }

    async UniTask Load_Script(string _asset_name)
    {
      // 에셋 로드.
      var lua_script = await AssetManager.Instance.LoadAssetAsync<LuaAsset>(_asset_name);            
      if (lua_script == null || string.IsNullOrEmpty(lua_script.Text))
        return;

      // 루아 스크립트 실행.
      await m_lua_state.DoStringAsync(lua_script.Text);

      // 에셋 언로드
      AssetManager.Instance.ReleaseAsset(_asset_name);
    }

    async UniTask RunScript(string _module_name, string _func_name)
    {
      if (m_lua_state.Environment.TryGetValue(_module_name, out var module_value) == false)
        return;

      if (module_value.TryRead<LuaTable>(out var module) == false)
        return;

      if (module.TryGetValue(_func_name, out var func_value) == false)
        return;

      if (func_value.TryRead<LuaFunction>(out var func) == false)
        return;

      await m_lua_state.CallAsync(func, Array.Empty<LuaValue>());
    }

    public async UniTask LoadAndRunScript(string _asset_name)
    {
      await Load_Default_Script();

      await Load_Script(_asset_name);

      if (m_lua_state.Environment["SCRIPT_MODULE"].TryRead<string>(out var module_name)
      &&  m_lua_state.Environment["SCRIPT_MODULE_FUNC"].TryRead<string>(out var func_name))
      {
          // 스크립트 실행 전 변수 초기화.
          m_lua_state.Environment["SCRIPT_MODULE"]      = LuaValue.Nil;
          m_lua_state.Environment["SCRIPT_MODULE_FUNC"] = LuaValue.Nil;

          // 스크립트 실행.
          await RunScript(module_name, func_name);

      }
      else
      {
         // 스크립트 실행 실패.
         Debug.LogError($"Failed to run script {_asset_name}");
      }
    }
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

    public static void SetLuaValue(this LuaTable _table, string _key, int _value)         => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, Int64 _value)       => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, LuaFunction _value) => _table[_key] = _value;
    public static void SetLuaValue(this LuaTable _table, string _key, LuaTable _value)    => _table[_key] = _value;

}
