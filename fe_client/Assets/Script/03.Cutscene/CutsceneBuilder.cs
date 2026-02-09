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
   // public static CutsceneSequence BuildCutscene(string _cutscene_name)
   // {
   //    return new CutsceneSequence();
   // }

   public static void CreateRoot()
   {
      if (Root != null)
      {
         Debug.LogError("CutsceneBuilder: Root already created.");
         return;
      }

      Root = new CutsceneSequence();
   }

   public static void CreateTrack()
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

   public static void FinishTrack()
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


   public static void Build(string _cutscene_name)
   {
      if (Root == null)
      {
         Debug.LogError("CutsceneBuilder: Root not created.");
         return;
      }

      if (string.IsNullOrEmpty(_cutscene_name))
      {
         Debug.LogError("CutsceneBuilder: Cutscene name is empty.");
         return;
      }

      CutsceneManager.Instance.RegisterCutscene(_cutscene_name, Root);
      Root  = null;
      Track = null;
   }



   public static void AddCutscene(Cutscene _cutscene)
   {
      if (Track == null)
      {
         Debug.LogError("CutsceneBuilder: Track not created.");
         return;
      }

      Track.AddCutscene(_cutscene);
   }

   public static void AddCutscene_Dialogue(DIALOGUE_SEQUENCE _dialogue_sequence)
   {
      AddCutscene(new Cutscene_Dialogue(Root, _dialogue_sequence));      
   }

   public static void AddCutscene_VFX_TileSelect(int _vfx_index, bool _create, (int x, int y) _position)
   {
      AddCutscene(new Cutscene_VFX_TileSelect(Root, _vfx_index, _create, _position));      
   }

   public static void AddCutscene_Trigger(int _trigger_id, bool _is_wait)
   {
      AddCutscene(new Cutscene_Trigger(Root, _trigger_id, _is_wait));      
   }

   public static void AddCutscene_Unit_Move(Int64 _unit_id, (int x, int y) _start_position, (int x, int y) _end_position)
   {
      AddCutscene(new Cutscene_Unit_Move(Root, _unit_id, _start_position, _end_position));      
   }



}