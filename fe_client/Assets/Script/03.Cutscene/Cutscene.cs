using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;


public enum EnumCutsceneType
{
    None      = 0,

    Wait      = 1,   // 대기

    Dialogue  = 10,  // 대화 연출
    Unit_Move = 20,
}

public abstract class Cutscene
{
   public abstract EnumCutsceneType Type { get; }
   public async UniTask Execute(CancellationToken _skip_token)
   {
      try
      {
         OnEnter();

         if (_skip_token.IsCancellationRequested)
            return;            

         await OnUpdate(_skip_token);
      }
      catch (OperationCanceledException)
      {
         // 취소 처리 발생하여도 계속 진행하기 위함.
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
   public string Name { get; private set; } = string.Empty;

}

public class CutsceneSequence
{

}


// public class Cutscene
// {
//    // List<Cutscene_Action> Actions
// }