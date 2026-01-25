using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

// [EventReceiver(
//    typeof(Dialogue_CompleteEvent)
//    )]
public class CutsceneManager : Singleton<CutsceneManager>//, IEventReceiver
{
   
   private Dictionary<string, CutsceneSequence> m_repository       = new();
   private Queue<string>                        m_queue_cutscene   = new();
   private string                               m_playing_cutscene = string.Empty;
   private CancellationTokenSource              m_cancel_token_source = null;


   public bool IsPlayingCutscene 
   {
      get
      {
         if (m_queue_cutscene.Count > 0)
            return true;

         if (string.IsNullOrEmpty(m_playing_cutscene) == false)
            return true;

         return false;
      }
   }

   


    protected override void OnLoop()
    {
      base.OnLoop();

      // 대기중인 컷씬이 있을 경우, 재생.
      if (m_queue_cutscene.Count > 0 && string.IsNullOrEmpty(m_playing_cutscene))
      {        
         var cutscene_name = m_queue_cutscene.Dequeue();

         // 비동기 실행이므로 Forget 처리.
         PlayCutscene(cutscene_name).Forget();
      }

    }

    // 컷씬 재생 요청.
    public void RequestPlayCutscene(string _cutscene_name) 
    { 
      if (m_repository.ContainsKey(_cutscene_name) == false)
      {
         // 이미 repository에 존재해야 한다.?
         Debug.LogError($"CutsceneManager: RequestPlayCutscene failed to find cutscene. {_cutscene_name}");
      }

      // 대기중인 컷씬 목록에 추가.
      m_queue_cutscene.Enqueue(_cutscene_name);
    }
    
    public void RegisterCutscene(string _cutscene_name, CutsceneSequence _cutscene)
    {
      if (string.IsNullOrEmpty(_cutscene_name))
         return;

      if (_cutscene == null)
         return;

      m_repository[_cutscene_name] = _cutscene;
    }


    private async UniTask PlayCutscene(string _cutscene_name)
    {
      if (string.IsNullOrEmpty(_cutscene_name))
         return;

      if (m_repository.TryGetValue(_cutscene_name, out var cutscene) == false)
         return;


      DisposeCancelToken();
      m_playing_cutscene    = _cutscene_name;
      m_cancel_token_source = new CancellationTokenSource();      

      // 컷씬 재생.
      await cutscene.Play(m_cancel_token_source.Token);

      DisposeCancelToken();         
      m_playing_cutscene = string.Empty;
    }


    private void DisposeCancelToken()
    {
      if (m_cancel_token_source != null)
      {
         m_cancel_token_source.Dispose();
         m_cancel_token_source = null;
      }
    }
}