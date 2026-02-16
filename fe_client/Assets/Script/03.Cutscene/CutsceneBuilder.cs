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
   public static CutsceneSequence Root  { get; private set; } = null;
   public static CutsceneTrack    Track { get; private set; } = null;
   public static string           Name  { get; private set; } = string.Empty;
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
   public static void AddCutscene_VFX_TileSelect(int _vfx_index, bool _create, (int x, int y) _position)
   {
      AddCutscene(new Cutscene_VFX_TileSelect(Root, _vfx_index, _create, _position));      
   }

   // 카메라 포커스 컷씬 추가.
   public static void AddCutscene_Camera_Position((int x, int y) _position)
   {
      AddCutscene(new Cutscene_Camera_Position(Root, _position));      
   }

   // 트리거 컷씬 추가.
   public static void AddCutscene_Trigger(int _trigger_id, bool _is_wait)
   {
      AddCutscene(new Cutscene_Trigger(Root, _trigger_id, _is_wait));      
   }

   // 유닛 이동 컷씬 추가.
   public static void AddCutscene_Unit_Move(List<UNIT_MOVE_DATA> _unit_move_data, bool _update_cell_position)
   {
      AddCutscene(new Cutscene_Unit_Move(Root, _unit_move_data, _update_cell_position));      
   }

   // Delay 컷씬 추가.
   public static void AddCutscene_Delay(float _wait_time)
   {
      AddCutscene(new Cutscene_Delay(Root, _wait_time));      
   }

   // 그리드 커서 컷씬 추가.
   public static void AddCutscene_Grid_Cursor((int x, int y) _position)
   {
      AddCutscene(new Cutscene_Grid_Cursor(Root, _position));      
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

      Root.AddCondition(_condition);
   }

   public static void AddCondition_PlayOneShot()
   {
      AddCondition(new CutsceneCondition_PlayOneShot());
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


}