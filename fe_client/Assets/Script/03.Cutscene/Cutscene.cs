using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class CutsceneCondition
{
   public abstract bool Verify(CutsceneSequence _sequence);
}

public abstract class Cutscene
{
   // public abstract EnumCutsceneType Type     { get; }
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
   private List<CutscenePlayEvent> PlayEvents { get; set; } = new();
   private List<CutsceneCondition> Conditions { get; set; } = new();
   private List<CutsceneTrack>     Tracks     { get; set; } = new();       
   public  BaseContainer           Memory     { get; private set; } = new();

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

   public void AddCondition(CutsceneCondition _condition)
   {
      if (_condition == null)
         return;

      Conditions.Add(_condition);
   }
   public void AddPlayEvent(CutscenePlayEvent _event)
   {
      PlayEvents.Add(_event);
   }

   public bool VerifyConditions()
   {
      // 재생중인 컷씬은... 당연히 막아야겠지?
      if (IsPlaying)
         return false;

      // 조건 검사.
      foreach (var condition in Conditions)
      {
         if (!condition.Verify(this))
            return false;
      }

      return true;
   }
}


// public class Cutscene
// {
//    // List<Cutscene_Action> Actions
// }