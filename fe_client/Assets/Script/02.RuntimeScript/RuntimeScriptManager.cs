using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Lua.Standard;
using Lua.Unity;
using Battle;
using Cysharp.Threading.Tasks;
#pragma warning disable CS1998 

public class RuntimeScriptManager : SingletonMono<RuntimeScriptManager>
{
    private LuaState                     m_lua_state  = null;

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
        RegisterEnum<EnumCutsceneType>();

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

        m_lua_tables.Clear();
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
        var is_registered = GetLuaValue<int>("RegisterFunction", "All");
        if (is_registered == 1)
        {
          Debug.Log("Tag 함수 이미 등록됨");
          return;
        }


        RegisterFunction_Entity();
        RegisterFunction_Tag();
        RegisterFunction_Cutscene();


        SetLuaValue("RegisterFunction", "All", 1);
    }

    void RegisterFunction_Entity()
    {
        SetLuaValue("EntityManager", "GetPosition", new LuaFunction(async (context, ct) =>
        {
            var entity_id = context.GetArgument<Int64>(0);
            var entity = EntityManager.Instance.GetEntity(entity_id);
            if (entity == null)
              return context.Return(LuaValue.Nil, LuaValue.Nil);

            return context.Return(entity.Cell.x, entity.Cell.y);
        }));
    }

    void RegisterFunction_Tag()
    {
        // TagManager SetTag 함수 등록.
        SetLuaValue("TagManager", "SetTag", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_DATA tag_data);
            TagManager.Instance.SetTag(tag_data);
            return context.Return();
        }));
    }

    void RegisterFunction_Cutscene()
    {
        SetLuaValue("CutsceneBuilder", "RootBegin", new LuaFunction(async (context, ct) =>
        {
            CutsceneBuilder.RootBegin(context.GetArgument<string>(0));
            return context.Return();
        }));

        SetLuaValue("CutsceneBuilder", "RootEnd", new LuaFunction(async (context, ct) =>
        {
            CutsceneBuilder.RootEnd();
            return context.Return();
        }));

        SetLuaValue("CutsceneBuilder", "TrackBegin", new LuaFunction(async (context, ct) =>
        {
            CutsceneBuilder.TrackBegin();
            return context.Return();
        }));

        SetLuaValue("CutsceneBuilder", "TrackEnd", new LuaFunction(async (context, ct) =>
        {
            CutsceneBuilder.TrackEnd();
            return context.Return();
        }));
        

        SetLuaValue("CutsceneBuilder", "AddCutscene_Dialogue", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out DIALOGUE_SEQUENCE dialogue_sequence);
            CutsceneBuilder.AddCutscene_Dialogue(dialogue_sequence);
            return context.Return();
        }));

        SetLuaValue("CutsceneBuilder", "AddCutscene_VFX_TileSelect", new LuaFunction(async (context, ct) =>
        {
            var vfx_index = context.GetArgument<int>(0);
            var create    = context.GetArgument<bool>(1);
            var pos_x     = context.GetArgument<int>(2);
            var pos_y     = context.GetArgument<int>(3);
            
            CutsceneBuilder.AddCutscene_VFX_TileSelect(vfx_index, create, (pos_x, pos_y));
            return context.Return();
        }));


        SetLuaValue("CutsceneBuilder", "AddCutscene_Trigger", new LuaFunction(async (context, ct) =>
        {
            var trigger_id = context.GetArgument<int>(0);
            var is_wait    = context.GetArgument<bool>(1);
            CutsceneBuilder.AddCutscene_Trigger(trigger_id, is_wait);
            return context.Return();
        }));

        SetLuaValue("CutsceneBuilder", "AddCutscene_Unit_Move", new LuaFunction(async (context, ct) =>
        {
            var unit_id   = context.GetArgument<Int64>(0);
            var start_pos_x = context.GetArgument<int>(1);
            var start_pos_y = context.GetArgument<int>(2);
            var end_pos_x   = context.GetArgument<int>(3);
            var end_pos_y   = context.GetArgument<int>(4);
            CutsceneBuilder.AddCutscene_Unit_Move(unit_id, (start_pos_x, start_pos_y), (end_pos_x, end_pos_y));
            return context.Return();
        }));

    }

    async UniTask Load_Default_Script()
    {
        // 기본 스크립트 로드 완료 여부 확인.
        var is_loaded = GetLuaValue<int>("Load_Default_Script", "IsLoaded");
        if (is_loaded == 1)
        {
          Debug.Log("기본 스크립트 이미 로드됨");
          return;
        }

        await Load_Script("runtimescript/common");
        await Load_Script("runtimescript/tag");    

        // 기본 스크립트 로드 완료.
        SetLuaValue("Load_Default_Script", "IsLoaded", 1);    
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

