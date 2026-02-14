using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

// TODO: 컷씬 타입이 뭔가 굳이 필요하지 않은 것 같기도?
public enum EnumCutsceneType
{
    None      = 0,

   //  Wait           = 1,   // 대기

    Dialogue       = 10,  // 대화 연출


    Unit_Move      = 20,  // 유닛 이동 연출. 

    Unit_Active    = 21,  // 유닛 표시 On/Off.



    VFX_TileSelect = 100,  // 타일 선택 커서 연출.

    Trigger        = 1000, // 트리거 관련 동작.
}


public enum EnumCutsceneLocalMemory
{
    // 컷씬에서 표시할 타일 선택 커서 VFX ID.
    VFX_Tile_Select_Begin = 100,
    VFX_Tile_Select_End   = 110, 
}


public enum EnumCutsceneGlobalMemory
{
   Trigger_Begin = 1,
   Trigger_End   = 1_000_000,
}


public abstract class Cutscene
{
   public abstract EnumCutsceneType Type     { get; }
   protected       CutsceneSequence Sequence { get; private set; }

   protected Cutscene(CutsceneSequence _sequence)
   {
      Sequence = _sequence;
   }




   public async UniTask Execute(CancellationToken _skip_token)
   {
      try
      {
         OnEnter();

         if (_skip_token.IsCancellationRequested == false)
            await OnUpdate(_skip_token);

      }
      catch (OperationCanceledException ex)
      {
         // 취소 처리 발생하여도 계속 진행하기 위함.
         Debug.LogWarning($"Cutscene Execute OperationCanceledException, {ex.Message}");
      }
      finally
      {
         OnExit();
      }
   }

   protected abstract void    OnEnter();
   protected abstract UniTask OnUpdate(CancellationToken _skip_token);
   protected abstract void    OnExit();
}


public class CutsceneTrack
{
   private List<Cutscene> Cutscenes { get; set; } = new();

   public async UniTask Play(CancellationToken _skip_token)
   {
      // 순차적으로 진행.
      foreach (var e in Cutscenes)
      {
         await e.Execute(_skip_token);
      }
   }

   public void AddCutscene(Cutscene _cutscene)
   {
      if (_cutscene == null)
         return;

      Cutscenes.Add(_cutscene);
   }
}

public class CutsceneSequence
{
   private List<CutsceneTrack> Tracks     { get; set; }         = new();

   public  BaseContainer       Memory { get; private set; } = new();

   public  bool IsPlaying { get; private set; } = false; // 연출이 진행중인지 체크.
   public  bool HasPlayed { get; private set; } = false; // 연출이 한번 실행되었는지 체크.

   public async UniTask Play(CancellationToken _skip_token)
   {
      IsPlaying = true;
      Debug.Log($"CutsceneSequence: Play start");
      // 병렬로 진행.
      await UniTask.WhenAll(Tracks.Select(e => e.Play(_skip_token)));
      IsPlaying = false;
      HasPlayed = true;
      Debug.Log($"CutsceneSequence: Play complete");
   }


   public void AddTrack(CutsceneTrack _track)
   {
      if (_track == null)
         return;

      Tracks.Add(_track);
   }
}


// public class Cutscene
// {
//    // List<Cutscene_Action> Actions
// }