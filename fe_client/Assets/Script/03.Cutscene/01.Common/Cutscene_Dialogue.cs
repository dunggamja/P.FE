using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lua;
using Battle;
using Cysharp.Threading.Tasks;
using System.Threading;
using R3;

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


   public bool              IsActive;
   public EnumPosition      Position;
   public DIALOGUE_PORTRAIT Portrait;
   public string            Dialogue;
}

public struct DIALOGUE_SEQUENCE
{
   public Int64 ID;
   public bool  CloseDialogue;
   public Queue<DIALOGUE_DATA> DialogueData;

   public void SetID(Int64 _id)
   {
      // 입력된 ID가 없으면 자동생성.
      if (_id == 0)
          _id = Util.GenerateID();

      ID = _id;
   }

   public void SetCloseDialogue(bool _close_dialogue)
   {
      CloseDialogue = _close_dialogue;
   }


   public void AddDialogueData(List<DIALOGUE_DATA> _dialogue_data)
   {
      if (DialogueData == null)
          DialogueData = new();

      foreach (var e in _dialogue_data)
      {
         DialogueData.Enqueue(e);
      }
   }

   public void AddDialogueData(Queue<DIALOGUE_DATA> _dialogue_data)
   {
      if (DialogueData == null)
          DialogueData = new();
   
      if (_dialogue_data != null)
      {
         foreach (var e in _dialogue_data)
         {
            DialogueData.Enqueue(e);
         }
      }

   }

   public bool HasDialogueData()
   {
      return DialogueData != null && DialogueData.Count > 0;
   }

   public DIALOGUE_DATA DequeueDialogueData()
   {
      if (HasDialogueData() == false)
         return default(DIALOGUE_DATA);

      return DialogueData.Dequeue();
   }
  


   public void Reset()
   {
      ID = 0;
      CloseDialogue = false;

      if (DialogueData != null)
          DialogueData.Clear();
   }
}



public class DialoguePublisher
{
   private static Subject<DIALOGUE_SEQUENCE> s_dialogue_sequence    = new();
   private static Subject<Int64>             s_complete_dialogue_id = new();

   public static void PublishDialogueSequence(DIALOGUE_SEQUENCE _dialogue_sequence)
   {
      s_dialogue_sequence.OnNext(_dialogue_sequence);
   }

   public static Observable<DIALOGUE_SEQUENCE> GetObserverDialogueSequence()
   {
      return s_dialogue_sequence.AsObservable();
   }


   public static void PublishComplete(Int64 _id)
   {
      s_complete_dialogue_id.OnNext(_id);
   }

   public static Observable<Int64> GetObserverComplete(Int64 _id)
   {
      return s_complete_dialogue_id.Where(e => e == _id).Take(1);
   }
}


// [EventReceiver(typeof(Dialogue_CompleteEvent))]
public class Cutscene_Dialogue : Cutscene//, IEventReceiver
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


      // GUI가 열릴때까지 대기. (최대 30초 간 대기 후 에러 처리.)
      await UniTask.WaitUntil(() 
         => 
         GUIManager.Instance.IsOpenUI(gui_name) == GUIManager.EnumGUIOpenState.Open, 
         cancellationToken: _skip_token)
         .Timeout(TimeSpan.FromSeconds(30));      


      // 대화 완료 이벤트 구독.
      var observer = DialoguePublisher.GetObserverComplete(m_dialogue_data.ID);

      // 대화 시퀀스 발행.
      DialoguePublisher.PublishDialogueSequence(m_dialogue_data);
    
      // 대화 완료 이벤트 대기.
      await observer.WaitAsync(_skip_token);

      Debug.Log($"Cutscene_Dialogue: Dialogue complete {m_dialogue_data.ID}");

   }

   protected override void OnExit()
   {
     
   }

}