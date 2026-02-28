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
        SetLuaValue("EntityManager", "GetEntity", new LuaFunction(async (context, ct) =>
        {
            // 태그로 인해 어떤식으로 찾기 요청이 들어올지 생각해보자.
            // 1. Entity, Entity_Faction, Entity_All, Entity_Group, 하위 계층 탐색하여서 전달하면 된다.
            // 2. 뭔가 속성에 대한 EnumTagAttributeType가 들어오면...? 그건 상위계층 탐색해서 주면 되겠군...?
            // 그럼 여기서는 하위계층 탐색해서 주면 끝.

            // 태그 정보.
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO tag_info);
            

            using var list_collect = ListPool<Entity>.AcquireWrapper();
            TagHelper.Collect_Entity(tag_info, list_collect.Value);

            // 엔티티 타입인지 체크하고, 반환값 셋팅.
            var lua_table       = new LuaTable();
            var lua_table_index = 1;
            foreach(var e in list_collect.Value)
            {
                lua_table[lua_table_index++] = e.ID;
            }

            return context.Return(lua_table);
        }));

        // 유닛 삭제.
        SetLuaValue("EntityManager", "RemoveEntity", new LuaFunction(async (context, ct) =>
        {
            return context.Return();
        }));

        // 유닛 생성.
        SetLuaValue("EntityManager", "CreateEntity", new LuaFunction(async (context, ct) =>
        {
            return context.Return();
        }));


        SetLuaValue("EntityManager", "GetPosition", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO tag_info);

            using var list_collect = ListPool<Entity>.AcquireWrapper();
            TagHelper.Collect_Entity(tag_info, list_collect.Value);

            // 엔티티 타입인지 체크하고, 반환값 셋팅.
            var lua_table       = new LuaTable();
            var lua_table_index = 1;
            foreach(var e in list_collect.Value)
            {
                var entity_position   = new LuaTable();
                entity_position["id"] = e.ID;
                entity_position["x"]  = e.Cell.x;
                entity_position["y"]  = e.Cell.y;

                lua_table[lua_table_index++] = entity_position;
            }
            
            return context.Return(lua_table);
        }));


        SetLuaValue("EntityManager", "SetCommandPriority", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO tag_info);
            var priority  = context.GetArgument<int>(1);
            
            using var list_collect = ListPool<Entity>.AcquireWrapper();
            TagHelper.Collect_Entity(tag_info, list_collect.Value);
            foreach(var entity in list_collect.Value)
            {
                entity.SetCommandPriority(priority);
            }

            return context.Return();
        }));




        // 



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
#region base
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
#endregion base

#region cutscene_action
        // 대화창 시작
        SetLuaValue("CutsceneBuilder", "Dialogue", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out DIALOGUE_SEQUENCE dialogue_sequence);
            CutsceneBuilder.AddCutscene_Dialogue(dialogue_sequence);
            return context.Return();
        }));


        // 대화창 종료
        SetLuaValue("CutsceneBuilder", "DialogueEnd", new LuaFunction(async (context, ct) =>
        {
            var dialogue = new DIALOGUE_SEQUENCE()
            {
                ID            = Util.GenerateID(),
                CloseDialogue = true,
                DialogueData  = null
            };
            
            CutsceneBuilder.AddCutscene_Dialogue(dialogue);
            return context.Return();
        }));

        // 타일 선택 및 카메라 포커스.
        SetLuaValue("CutsceneBuilder", "VFX_TileSelect_On", new LuaFunction(async (context, ct) =>
        {
            var vfx_index = context.GetArgument<int>(0);
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(1), out TAG_INFO tag);
            
            CutsceneBuilder.AddCutscene_VFX_TileSelect(vfx_index, true, tag);
            return context.Return();
        }));

        // 타일 선택 OFF
        SetLuaValue("CutsceneBuilder", "VFX_TileSelect_Off", new LuaFunction(async (context, ct) =>
        {
            var vfx_index = context.GetArgument<int>(0);
            
            CutsceneBuilder.AddCutscene_VFX_TileSelect(vfx_index, false, default);
            return context.Return();
        }));

        // 카메라 포커스 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Camera_Position", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO tag);var pos_y = context.GetArgument<int>(1);
            CutsceneBuilder.AddCutscene_Camera_Position(tag);
            return context.Return();
        }));


        // 글로벌 트리거 SET
        SetLuaValue("CutsceneBuilder", "GlobalTriggerSet", new LuaFunction(async (context, ct) =>
        {
            var trigger_id = context.GetArgument<int>(0);
            CutsceneBuilder.AddCutscene_Trigger(trigger_id, _is_set: true, _is_local: false);
            return context.Return();
        }));

        // 로컬 트리거 SET
        SetLuaValue("CutsceneBuilder", "LocalTriggerSet", new LuaFunction(async (context, ct) =>
        {
            var trigger_id = context.GetArgument<int>(0);
            CutsceneBuilder.AddCutscene_Trigger(trigger_id, _is_set: true, _is_local: true);
            return context.Return();
        }));

        // 글로벌 트리거 WAIT
        SetLuaValue("CutsceneBuilder", "GlobalTriggerWait", new LuaFunction(async (context, ct) =>
        {
            var trigger_id = context.GetArgument<int>(0);
            CutsceneBuilder.AddCutscene_Trigger(trigger_id, _is_set: false, _is_local: false);
            return context.Return();
        }));

        // 로컬 트리거 WAIT
        SetLuaValue("CutsceneBuilder", "LocalTriggerWait", new LuaFunction(async (context, ct) =>
        {
            var trigger_id = context.GetArgument<int>(0);
            CutsceneBuilder.AddCutscene_Trigger(trigger_id, _is_set: false, _is_local: true);
            return context.Return();
        }));

        // 유닛 이동 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Unit_Move", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out List<UNIT_MOVE_DATA> unit_move_data);
            var update_cell_position = context.GetArgument<bool>(1);

            CutsceneBuilder.AddCutscene_Unit_Move(unit_move_data, update_cell_position);
            return context.Return();
        }));

        // 유닛 표시 On/Off 추가.
        SetLuaValue("CutsceneBuilder", "Unit_Show", new LuaFunction(async (context, ct) =>
        {
            var table = context.GetArgument<LuaTable>(0);            
            var show  = context.GetArgument<bool>(1);

            RuntimeScriptHelper.FromLua(table, out List<Int64> list_unit_id);
            CutsceneBuilder.AddCutscene_Unit_Show(list_unit_id, show);
            return context.Return();
        }));

        
        SetLuaValue("CutsceneBuilder", "Unit_AIType", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO tag_info);
            var ai_type = context.GetArgument<int>(1);

            using var list_collect = ListPool<Entity>.AcquireWrapper();
            using var list_unit_id = ListPool<Int64>.AcquireWrapper();
            TagHelper.Collect_Entity(tag_info, list_collect.Value);

            foreach(var e in list_collect.Value)
            {
                list_unit_id.Value.Add(e.ID);
            }            
            
            CutsceneBuilder.AddCutscene_Unit_AIType(list_unit_id.Value, (EnumAIType)ai_type);

            
            return context.Return();
        }));

        // Delay 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Delay", new LuaFunction(async (context, ct) =>
        {
            var delay_time = context.GetArgument<float>(0);
            CutsceneBuilder.AddCutscene_Delay(delay_time);
            return context.Return();
        }));

        // 그리드 커서 컷씬 추가.
        SetLuaValue("CutsceneBuilder", "Grid_Cursor", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO tag);
            CutsceneBuilder.AddCutscene_Grid_Cursor(tag);
            return context.Return();
        }));


#endregion cutscene_action

#region cutscene_condition
        // // 한번만 실행되도록 조건 추가.
        // SetLuaValue("CutsceneBuilder", "Condition_PlayOneShot", new LuaFunction(async (context, ct) =>
        // {
        //     CutsceneBuilder.AddCondition_PlayOneShot();
        //     return context.Return();
        // }));

        // 공격 또는 방어유닛에 대상 유닛이 존재하는지 체크합니다.
        SetLuaValue("CutsceneBuilder", "Condition_Combat_Unit", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO unit_tag);
            CutsceneBuilder.AddCondition_Combat_Unit(unit_tag);
            return context.Return();
        }));

        // 공격 또는 방어유닛에 대상 유닛이 존재하는지 체크합니다.
        SetLuaValue("CutsceneBuilder", "Condition_Combat_Unit_Dead", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out TAG_INFO unit_tag);
            CutsceneBuilder.AddCondition_Combat_Unit_Dead(unit_tag);
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

#region cutscene_life_time
        SetLuaValue("CutsceneBuilder", "LifeTime", new LuaFunction(async (context, ct) =>
        {
            RuntimeScriptHelper.FromLua(context.GetArgument<LuaTable>(0), out CutsceneLifeTime life_time);
            CutsceneBuilder.SetLifeTime(life_time);
            return context.Return();
        }));
#endregion cutscene_life_time
    }
}
