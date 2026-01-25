using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;

public struct DIALOGUE_PORTRAIT
{
   public string Name;
   public string PortraitAsset;  // 2D
   public string PortraitSprite; // 2D
}

public struct DIALOGUE_DATA
{
   public enum EnumPosition
   {
      Top, Center, Bottom, Max
   }


   public bool         IsActive;
   public EnumPosition Position;
   public string       Name;
   public string       Dialogue;
}

public struct DIALOGUE_SEQUENCE
{
   public Int64               ID;
   public Queue<DIALOGUE_DATA> DialogueData;

   public void SetID(Int64 _id)
   {
      ID = _id;
   }

   public void AddDialogueData(List<DIALOGUE_DATA> _dialogue_data)
   {
      foreach (var e in _dialogue_data)
      {
         DialogueData.Enqueue(e);
      }
   }

   public void AddDialogueData(Queue<DIALOGUE_DATA> _dialogue_data)
   {
      foreach (var e in _dialogue_data)
      {
         DialogueData.Enqueue(e);
      }
   }

   public void Reset()
   {
      ID = 0;
      DialogueData.Clear();
   }
}


[EventReceiver(typeof(Dialogue_CompleteEvent))]
public class Cutscene_Dialogue : Cutscene, IEventReceiver
{
   public override EnumCutsceneType Type => EnumCutsceneType.Dialogue;
   private DIALOGUE_SEQUENCE m_dialogue_data = new ();
   private bool              m_dialogue_done = false;

   public Cutscene_Dialogue(CutsceneSequence _sequence, DIALOGUE_SEQUENCE _dialogue_data) : base(_sequence)
   {
      m_dialogue_data = _dialogue_data;
   }


   protected override void OnEnter()
   {
   }

   protected override async UniTask OnUpdate(CancellationToken _skip_token)
   {
      var gui_name      = GUIPage_Dialogue.PARAM.Create().GUIName;
      var gui_not_exist = GUIManager.Instance.IsOpenUI(gui_name) == GUIManager.EnumGUIOpenState.None;           
      if (gui_not_exist) 
         GUIManager.Instance.OpenUI(GUIPage_Dialogue.PARAM.Create());


      // GUI가 열릴때까지 대기. (최대 10초 간 대기 후 에러 처리.)
      await UniTask.WaitUntil(() 
         => 
         GUIManager.Instance.IsOpenUI(gui_name) == GUIManager.EnumGUIOpenState.Open, 
         cancellationToken: _skip_token)
         .Timeout(TimeSpan.FromSeconds(10));

      try
      {
         EventDispatchManager.Instance.AttachReceiver(this);

         // 대화 이벤트 발생.
         EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<Dialogue_RequestPlayEvent>.Acquire().Set(m_dialogue_data));

         // 대화 이벤트 종료까지 대기
         await UniTask.WaitUntil(() => m_dialogue_done, cancellationToken: _skip_token);
      }
      finally
      {
         EventDispatchManager.Instance.DetachReceiver(this);
      }
   }

   protected override void OnExit()
   {
      //throw new NotImplementedException();
   }

    public void OnReceiveEvent(IEventParam _event)
    {
        switch(_event)
        {
            case Dialogue_CompleteEvent dialogue_complete_event:
                OnReceiveEvent_Dialogue_CompleteEvent(dialogue_complete_event);
                break;
        }
    }

    void OnReceiveEvent_Dialogue_CompleteEvent(Dialogue_CompleteEvent _event)
    {
        if (_event == null)
            return;

        if (_event.DialogueID != m_dialogue_data.ID)
            return;

        m_dialogue_done = true;
    }
}