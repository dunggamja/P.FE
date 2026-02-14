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

   // 컷씬 추가.
   public static void AddCutscene(Cutscene _cutscene)
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
      AddCutscene(new Cutscene_Dialogue(Root, _dialogue_sequence));      
   }

   // 타일 선택 컷씬 추가.
   public static void AddCutscene_VFX_TileSelect(int _vfx_index, bool _create, (int x, int y) _position)
   {
      AddCutscene(new Cutscene_VFX_TileSelect(Root, _vfx_index, _create, _position));      
   }

   // 트리거 컷씬 추가.
   public static void AddCutscene_Trigger(int _trigger_id, bool _is_wait)
   {
      AddCutscene(new Cutscene_Trigger(Root, _trigger_id, _is_wait));      
   }

   // 유닛 이동 컷씬 추가.
   public static void AddCutscene_Unit_Move(Int64 _unit_id, (int x, int y) _start_position, (int x, int y) _end_position)
   {
      AddCutscene(new Cutscene_Unit_Move(Root, _unit_id, _start_position, _end_position));      
   }



}