using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class CutsceneBuilder
{
   public static CutsceneSequence  Root               { get; private set; } = null;
   public static CutsceneTrack     Track              { get; private set; } = null;

   public static CutsceneCondition_Decorator 
                                   ConditionDecorator { get; private set; } = null;
   public static string            Name               { get; private set; } = string.Empty;
   // public static CutsceneSequence BuildCutscene(string _cutscene_name)
   // {
   //    return new CutsceneSequence();
   // }

   // 컷씬 시작.
   public static void RootBegin(string _name)
   {
      if (Root != null)
      {
         Debug.LogError("CutsceneBuilder: Root already created.");
         return;
      }

      if (string.IsNullOrEmpty(_name))
      {
         Debug.LogError("CutsceneBuilder: Cutscene name is empty.");
         return;
      }

      Root = new CutsceneSequence();
      Name = _name;
   }

   // 컷씬 종료.
   public static void RootEnd()
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }

      if (string.IsNullOrEmpty(Name))
      {
         Debug.LogError("CutsceneBuilder: Cutscene name is empty.");
         return;
      }

      CutsceneManager.Instance.RegisterCutscene(Name, Root);
      Root  = null;
      Track = null;
   }

   // 트랙 시작.
   public static void TrackBegin()
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }

      if (Track != null)
      {
         Debug.LogError("CutsceneBuilder: Track already created.");
         return;
      }

      Track = new CutsceneTrack();
   }

   // 트랙 종료.
   public static void TrackEnd()
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }


      if (Track == null)
      {
         Debug.LogError("CutsceneBuilder: Track not created.");
         return;
      }

      Root.AddTrack(Track);
      Track = null;
   }

   private static void Decorator(CutsceneCondition_Decorator _decorator)
   {
      if (ConditionDecorator != null)
      {
         Debug.LogError("CutsceneBuilder: Decorated condition already created.");         
      }

      ConditionDecorator = _decorator;
   }


   public static void Decorator_Not()
   {
      Decorator(new CutsceneCondition_Decorator_Not());
   }





#region cutscene_action
   // 컷씬 추가.
   private static void AddCutscene(Cutscene _cutscene)
   {
      if (Track == null)
      {
         Debug.LogError("CutsceneBuilder: Track not created.");
         return;
      }


      Track.AddCutscene(_cutscene);
   }

   // 대화 컷씬 추가.
   public static void AddCutscene_Dialogue(DIALOGUE_SEQUENCE _dialogue_sequence)
   {
      // _dialogue_sequence.SetID(Util.GenerateID());
      AddCutscene(new Cutscene_Dialogue(Root, _dialogue_sequence));      
   }

   // 타일 선택 컷씬 추가.
   public static void AddCutscene_VFX_TileSelect(int _vfx_index, bool _create, TAG_INFO _tag)
   {
      AddCutscene(new Cutscene_VFX_TileSelect(Root, _vfx_index, _create, _tag));      
   }

   // 카메라 포커스 컷씬 추가.
   public static void AddCutscene_Camera_Position(TAG_INFO _tag)
   {
      AddCutscene(new Cutscene_Camera_Position(Root, _tag));      
   }

   // 트리거 컷씬 추가.
   public static void AddCutscene_Trigger(int _trigger_id, bool _is_set, bool _is_local)
   {
      if (_is_local)
         AddCutscene(new Cutscene_LocalTrigger(Root, _trigger_id, _is_set));
      else
         AddCutscene(new Cutscene_Trigger(Root, _trigger_id, _is_set));
   }

   // 유닛 이동 컷씬 추가.
   public static void AddCutscene_Unit_Move(List<UNIT_MOVE_DATA> _unit_move_data, bool _update_cell_position)
   {
      AddCutscene(new Cutscene_Unit_Move(Root, _unit_move_data, _update_cell_position));      
   }

   // 유닛 표시 On/Off 컷씬 추가.
   public static void AddCutscene_Unit_Show(List<Int64> _unit_id, bool _show)
   {
      AddCutscene(new Cutscene_Unit_Show(Root, _unit_id, _show));      
   }

   // 유닛 AI 타입 컷씬 추가.
   public static void AddCutscene_Unit_AIType(List<Int64> _unit_id, EnumAIType _ai_type)
   {
      AddCutscene(new Cutscene_Unit_AIType(Root, _unit_id, _ai_type));      
   }

   // 유닛 맵에서 이탈 처리.
   public static void AddCutscene_Unit_Exit(List<Int64> _unit_id)
   {
      AddCutscene(new Cutscene_Unit_Exit(Root, _unit_id));      
   }

   // Delay 컷씬 추가.
   public static void AddCutscene_Delay(float _wait_time)
   {
      AddCutscene(new Cutscene_Delay(Root, _wait_time));      
   }

   // 그리드 커서 컷씬 추가.
   public static void AddCutscene_Grid_Cursor(TAG_INFO _tag)
   {
      AddCutscene(new Cutscene_Grid_Cursor(Root, _tag));      
   }

#endregion cutscene_action


#region cutscene_condition
   // 조건 추가.
   private static void AddCondition(CutsceneCondition _condition)
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }

      // 데코레이터가 있을경우 처리.
      if (ConditionDecorator != null)
      {
          ConditionDecorator.SetCondition(_condition);

          _condition         = ConditionDecorator;
          ConditionDecorator = null;
      }




      Root.AddCondition(_condition);
   }

   private static void DecorateCondition()
   {

   }
   

   // 공격 또는 방어유닛에 대상 유닛이 존재하는지 체크합니다.
   public static void AddCondition_Combat_Unit(TAG_INFO _unit_tag)
   {
      AddCondition(new CutsceneCondition_Combat_Unit(_unit_tag));
   }

   // 전투에서 대상 유닛이 죽었는지 체크합니다.
   public static void AddCondition_Combat_Unit_Dead(TAG_INFO _unit_tag)
   {
      AddCondition(new CutsceneCondition_Combat_Unit_Dead(_unit_tag));
   }

   // 유닛과 대상 유닛의 거리가 조건에 맞는지 체크합니다.
   public static void AddCondition_Range(TAG_INFO _tag_1, TAG_INFO _tag_2, int _min_range, int _max_range)
   {
      AddCondition(new CutsceneCondition_Range(_tag_1, _tag_2, _min_range, _max_range));
   }

   // 트리거 존재 체크
   public static void AddCondition_Trigger(int _trigger_id, bool _has_trigger)
   {
      AddCondition(new CutsceneCondition_Trigger(_trigger_id, _has_trigger));
   }

   // 글로벌 블랙보드 조건 추가.
   public static void AddCondition_Blackboard(int _blackboard_key, EnumBlackBoardCompare _compare_type, Int64 _value = 0)
   {
      AddCondition(new CutsceneCondition_Blackboard(_blackboard_key, _compare_type, _value));
   }

   // 명령중인 유닛 조건 추가.
   public static void AddCondition_CommandEntity(TAG_INFO _tag)
   {
      AddCondition(new CutsceneCondition_CommandEntity(_tag));
   }

   // 유닛 조건 추가.
   public static void AddCondition_Entity(TAG_INFO _tag, EnumEntityCondition _condition)
   {
      AddCondition(new CutsceneCondition_Entity_Condition(_tag, _condition));
   }

   

#endregion cutscene_condition


#region cutscene_play_event
   private static void AddPlayEvent(CutscenePlayEvent _event)
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }

      Root.AddPlayEvent(_event);
   }

   public static void AddPlayEvent(EnumCutscenePlayEvent _event, Int64 _value1 = 0, Int64 _value2 = 0)
   {
      AddPlayEvent(CutscenePlayEvent.Create(_event, _value1, _value2));
   }
#endregion cutscene_play_event


#region cutscene_life_time
   public static void SetLifeTime(CutsceneLifeTime _life_time)
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }

      Root.SetLifeTime(_life_time);
   }
#endregion cutscene_life_time


#region cutscene_item
   public static void AddCutscene_Item(TAG_INFO _target, List<ItemData> _items, bool _acquire)
   {
      AddCutscene(new Cutscene_Item_Change(Root, _target, _items, _acquire));
   }
#endregion cutscene_item

}