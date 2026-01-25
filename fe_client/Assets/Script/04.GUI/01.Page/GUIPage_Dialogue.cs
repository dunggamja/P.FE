using System;
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Battle;


[EventReceiver(
   typeof(GUI_Menu_SelectEvent),
   typeof(Dialogue_RequestPlayEvent)
)]
public class GUIPage_Dialogue : GUIPage, IEventReceiver
{
   public enum EnumDialogueState
   {
      None,
      Playing,
      Complete,
   }

   public class PARAM : GUIOpenParam
   {
      public override EnumGUIType GUIType => EnumGUIType.Screen;

      private PARAM() 
        : base(
            // id      
            GUIPage.GenerateID(),    

            // asset path
            "gui/page/dialogue",   

            // is input enabled
            true,

            // is multiple open
            false                     
            )      
        { }

        static public PARAM Create()
        {
            return new PARAM();
        }
   }






   [SerializeField]
   private GUIElement_Dialogue[] m_dialogue = new GUIElement_Dialogue[(int)DIALOGUE_DATA.EnumPosition.Max];


   public  EnumDialogueState    DialogueState { get; private set; } = EnumDialogueState.None;

   private DIALOGUE_SEQUENCE    m_dialogue_data = new();

   
  


    public void OnReceiveEvent(IEventParam _event)
    {
       switch(_event)
       {
         case GUI_Menu_SelectEvent menu_select_event:
            OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
            break;
         case Dialogue_RequestPlayEvent dialogue_requet_play_event:
            OnReceiveEvent_Dialogue_RequetPlayEvent(dialogue_requet_play_event);
            break;
       }

    }

    protected override void OnOpen(GUIOpenParam _param)
    {
      // 모든 대화창 OFF. 요청이 올때 켜주도록 하자.
      foreach (var e in m_dialogue)
      {
         e.SetActive(false);
      }
    }

    protected override void OnLoop()
    {
        base.OnLoop(); 

        // 대기중인 대화가 있을 경우 재생.
        if (DialogueState != EnumDialogueState.Playing)
        {
            if (m_dialogue_data.DialogueData.Count > 0)
            {
               PlayDialogue(m_dialogue_data.DialogueData.Dequeue());
            }
        }
    }

    protected override void OnClose()
    {
      //   throw new NotImplementedException();
    }


    protected override void OnPostProcess_Close()
    {
      //   throw new NotImplementedException();
    }




    void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
      if (_event == null || _event.GUI_ID != ID)
         return;

      // TODO: 현재는 대화 재생만 있는데... 선택지 이벤트 구현도 필요할듯.

      // 대화 재생 중이면 스크롤 처리.
      ScrollPlayingDialogue();
    }

    void OnReceiveEvent_Dialogue_RequetPlayEvent(Dialogue_RequestPlayEvent _event)
    {
      if (_event == null)
         return;

      m_dialogue_data.Reset();
      m_dialogue_data.SetID(_event.DialogueSequence.ID);
      m_dialogue_data.AddDialogueData(_event.DialogueSequence.DialogueData);
    }


    void PlayDialogue(DIALOGUE_DATA _data)
    {
       // 
       var index = (int)_data.Position;
       if (index < 0 || index >= m_dialogue.Length)
          return;

       m_dialogue[index].SetDialogue(_data.Name, _data.Dialogue);
       DialogueState = EnumDialogueState.Playing;       
    }

    void ScrollPlayingDialogue()
    {
      // 재생중인 대화가 없으면 실행하지 않는다.
      if (DialogueState != EnumDialogueState.Playing)
         return;


      var scroll_result = false;
      foreach (var e in m_dialogue)
      {
         scroll_result |= e.ScrollToNextDialogue();
      }

      // 재생중인 대화를 끝까지 확인했다.
      if (scroll_result == false)
      {
         DialogueState = EnumDialogueState.Complete;

         EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<Dialogue_CompleteEvent>.Acquire().Set(m_dialogue_data.ID));
      }
    }

}
