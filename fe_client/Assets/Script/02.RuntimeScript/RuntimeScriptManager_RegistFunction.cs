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

public partial class RuntimeScriptManager 
{
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
        

#region cutscene_action
        // 대화 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Dialogue", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out DIALOGUE_SEQUENCE dialogue_sequence);
            CutsceneBuilder.AddCutscene_Dialogue(dialogue_sequence);
            return context.Return();
        }));

        // 타일 선택 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "VFX_TileSelect", new LuaFunction(async (context, ct) =>
        {
            var vfx_index = context.GetArgument<int>(0);
            var create    = context.GetArgument<bool>(1);
            var pos_x     = context.GetArgument<int>(2);
            var pos_y     = context.GetArgument<int>(3);
            
            CutsceneBuilder.AddCutscene_VFX_TileSelect(vfx_index, create, (pos_x, pos_y));
            return context.Return();
        }));


        // 트리거 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Trigger", new LuaFunction(async (context, ct) =>
        {
            var trigger_id = context.GetArgument<int>(0);
            var is_wait    = context.GetArgument<bool>(1);
            CutsceneBuilder.AddCutscene_Trigger(trigger_id, is_wait);
            return context.Return();
        }));

        // 유닛 이동 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Unit_Move", new LuaFunction(async (context, ct) =>
        {
            var unit_id     = context.GetArgument<Int64>(0);
            var start_pos_x = context.GetArgument<int>(1);
            var start_pos_y = context.GetArgument<int>(2);
            var end_pos_x   = context.GetArgument<int>(3);
            var end_pos_y   = context.GetArgument<int>(4);
            CutsceneBuilder.AddCutscene_Unit_Move(unit_id, (start_pos_x, start_pos_y), (end_pos_x, end_pos_y));
            return context.Return();
        }));
#endregion cutscene_action

#region cutscene_condition
        // 한번만 실행되도록 조건 추가.
        SetLuaValue("CutsceneBuilder", "Condition_PlayOneShot", new LuaFunction(async (context, ct) =>
        {
            CutsceneBuilder.AddCondition_PlayOneShot();
            return context.Return();
        }));
#endregion cutscene_condition

#region cutscene_play_event
        // 컷씬 이벤트 추가.
        SetLuaValue("CutsceneBuilder", "PlayEvent", new LuaFunction(async (context, ct) =>
        {
            var event_type = context.GetArgument<int>(0);
            var value1     = context.GetArgument<Int64>(1);
            var value2     = context.GetArgument<Int64>(2);
            CutsceneBuilder.AddPlayEvent((EnumCutscenePlayEvent)event_type, value1, value2);
            return context.Return();
        }));
#endregion cutscene_play_event
    }
}
