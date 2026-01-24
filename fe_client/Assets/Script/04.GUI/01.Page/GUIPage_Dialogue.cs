using System;
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Battle;


[EventReceiver(
   typeof(GUI_Menu_SelectEvent)
)]
public class GUIPage_Dialogue : GUIPage, IEventReceiver
{
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

   public enum EnumPosition
   {
      Top, Center, Bottom, Max
   }

   public enum EnumDialogueState
   {
      None,
      Playing,
      Complete,
   }

   [SerializeField]
   private GUIElement_Dialogue[] m_dialogue = new GUIElement_Dialogue[(int)EnumPosition.Max];

   


    public void OnReceiveEvent(IEventParam _event)
    {
       switch(_event)
       {
         case GUI_Menu_SelectEvent menu_select_event:
            OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
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

         foreach (var e in m_dialogue)
         {
            e.ScrollToNextDialogue();
         }
    }





}
