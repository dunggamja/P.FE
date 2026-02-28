using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;

// [EventReceiver(
//    typeof(Dialogue_CompleteEvent)
//    )]
public class CutsceneManager : Singleton<CutsceneManager>//, IEventReceiver
{
   
   private Dictionary<string, CutsceneSequence> m_repository          = new();
   private Dictionary<CutscenePlayEvent, HashSet<string>> 
                                                m_repository_by_event = new();
   private Queue<string>                        m_queue_cutscene      = new();
   private string                               m_playing_cutscene    = string.Empty;
   private CancellationTokenSource              m_cancel_token_source = null;

   public  BaseContainer                        Memory { get; private set; } = new();


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

    private CutsceneSequence GetCutscene(string _cutscene_name)
    {
      if (m_repository.TryGetValue(_cutscene_name, out var cutscene))
         return cutscene;

      return null;
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

      // // 컷씬 재생중에는 전투시스템을 정지합시다.
      // BattleSystemManager.Instance.SetPause(BattleSystemPauseReason.Cutscene, IsPlayingCutscene);

    }

    // 컷씬 재생 요청.
    public void RequestPlayCutscene(string _cutscene_name) 
    { 
      if (m_repository.ContainsKey(_cutscene_name) == false)
      {
         // 이미 repository에 존재해야 한다.?
         Debug.LogError($"CutsceneManager: RequestPlayCutscene failed to find cutscene. {_cutscene_name}");
         return;
      }

      // 재생 조건 검사.
      if (VerifyPlayCutscene(_cutscene_name) == false)
         return;

      // 대기중인 컷씬 목록에 추가.
      m_queue_cutscene.Enqueue(_cutscene_name);
    }

    public void OnPlayEvent(CutscenePlayEvent _event)
    {
        if (m_repository_by_event.TryGetValue(_event, out var list_cutscene) == false)
            return;

         foreach (var name in list_cutscene)
         {
            // var cutscene = GetCutscene(name);
            // if (cutscene == null)
            //    continue;
            // if (cutscene.PlayEvents.Any(e => e == _event) == false)
            //    continue;
            
            RequestPlayCutscene(name);
         }

    }


    
    public void RegisterCutscene(string _cutscene_name, CutsceneSequence _cutscene)
    {
      if (string.IsNullOrEmpty(_cutscene_name))
         return;

      if (_cutscene == null)
         return;

      // 컷씬 등록.
      m_repository[_cutscene_name] = _cutscene;

      // 컷씬 이벤트 호출 등록.
      foreach (var e in _cutscene.PlayEvents)
      {
         if (m_repository_by_event.TryGetValue(e, out var list_cutscene) == false)
         {
            list_cutscene = new HashSet<string>();
            m_repository_by_event.Add(e, list_cutscene);
         }

         list_cutscene.Add(_cutscene_name);
      }
    }

    void RemoveCutscene(string _cutscene_name)
    {
      if (string.IsNullOrEmpty(_cutscene_name))
         return;

      // 컷씬 목록에서 제거.
      m_repository.Remove(_cutscene_name);

      // 컷씬 이벤트 호출 등록 목록에서 제거.
      foreach (var e in m_repository_by_event)
      {
         e.Value.Remove(_cutscene_name);
      }
    }


    private async UniTask PlayCutscene(string _cutscene_name)
    {
      if (string.IsNullOrEmpty(_cutscene_name))
         return;

      if (m_repository.TryGetValue(_cutscene_name, out var cutscene) == false)
         return;

      if (cutscene.VerifyConditions() == false)
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

    public bool VerifyPlayCutscene(string _cutscene_name)
    {
      if (string.IsNullOrEmpty(_cutscene_name))
         return false;

      if (m_repository.TryGetValue(_cutscene_name, out var cutscene) == false)
         return false;

      return cutscene.VerifyConditions();
    }

    // 수명이 종료된 컷씬들을 제거합니다.
    // TODO: 언제 호출할지 고민해야봐야 함.
    public void CleanUpLifeTimeOver()
    {
       // 수명이 종료된 컷씬들을 수집합니다.
       using var list_remove = ListPool<string>.AcquireWrapper();
       foreach (var e in m_repository)
       {
         if (e.Value.VerifyLifeTime() == false)
            list_remove.Value.Add(e.Key);
       }

       // 수명이 종료된 컷씬들을 제거
       foreach (var name in list_remove.Value)
         RemoveCutscene(name);
    }
}