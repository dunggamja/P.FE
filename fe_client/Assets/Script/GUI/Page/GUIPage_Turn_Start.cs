
using System;
using System.Collections.Generic;
using Battle;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    // typeof(Battle_Scene_ChangeEvent)
)]
public class GUIPage_Turn_Start : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        public int Turn { get; private set; } = 0;

        private PARAM(int _turn) : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/turn_start",

            // is input enabled
            false,

            // is multiple open
            true
        )
        {
            Turn = _turn;
        }

        static public PARAM Create(int _turn)
        {
            return new PARAM(_turn);
        }
    }

    [SerializeField]
    private TextMeshProUGUI m_text_turn_number;


    private IDisposable     m_text_turn_number_subscription;


    public void OnReceiveEvent(IEventParam _event)
    {
        // throw new NotImplementedException();
    }



    protected override void OnOpen(GUIOpenParam _param)
    {
      
        var param = _param as PARAM;
        if (param == null)
        {
            Debug.LogError("GUIPage_Turn_Start: OnOpen param is null");
            return;
        }

        var localize_key     = LocalizeKey.Create("localization_base", "ui_turn_number");
        var localize_subject = LocalizationManager.Instance.GetTextObservable(
                localize_key.Table, 
                localize_key.Key);


        // 턴 number
        var turn = param.Turn;

        // 턴 표시
        m_text_turn_number_subscription = localize_subject
          .Subscribe(text => 
          {
            if (m_text_turn_number)
                m_text_turn_number.text = string.Format(text, turn);
          });


          // 2초 뒤 자동 닫기.
          Invoke(nameof(CloseSelf), 2.0f);
    }

    protected override void OnClose()
    {
        // throw new NotImplementedException();
        m_text_turn_number_subscription?.Dispose();
        m_text_turn_number_subscription = null;
    }

    protected override void OnPostProcess_Close()
    {
        // throw new NotImplementedException();
    }

    
}