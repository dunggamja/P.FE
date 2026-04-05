using System;
using Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// 전투 결과 표시
/// TODO: 프리팹 생성, 승리 패배에 따른 기능
/// 승리 => 전투 모드 종료 후 다음 챕터로.
/// 패배 => 전투 모드 종료 후 선택지 표시 (타이틀로 돌아가기, 데이터 로드하기)
public class GUIPage_Battle_Result : GUIPage
{
    public class PARAM : GUIOpenParam
    {
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        public bool    IsVictory   { get; private set; }
        // public Action  OnPrimary   { get; private set; }

        private PARAM(bool _is_victory)
            : base(
                GUIPage.GenerateID(),
                "gui/page/battle_result",
                true,
                false)
        {
            IsVictory = _is_victory;
        }

        public static PARAM Create(EnumBattleResult _result)
        {
            return new PARAM(_result == EnumBattleResult.Victory);
        }
    }

    // [SerializeField] Button         m_button_primary;
    [SerializeField] TextMeshProUGUI   m_text_title;

    // Action m_on_primary;

    protected override void OnOpen(GUIOpenParam _param)
    {
        var p = _param as PARAM;
        if (p == null)
        {
            Debug.LogError("GUIPage_Battle_Result: invalid param");
            return;
        }

        // m_on_primary = p.OnPrimary;

        if (m_text_title != null)
            m_text_title.text = p.IsVictory ? "Victory" : "Defeat";

        // if (m_button_primary != null)
        // {
        //     m_button_primary.onClick.RemoveAllListeners();
        //     m_button_primary.onClick.AddListener(OnClickPrimary);
        // }
    }

    // void OnClickPrimary()
    // {
    //     // var cb = m_on_primary;
    //     // m_on_primary = null;

    //     // if (m_button_primary != null)
    //     //     m_button_primary.onClick.RemoveAllListeners();

    //     // cb?.Invoke();
    //     CloseSelf();
    // }

    protected override void OnClose()
    {
    }

    protected override void OnPostProcess_Close()
    {
    }

    /// <summary>나중에 입력을 켜도, 포커스를 잃었다고 결과 UI를 숨기지 않음.</summary>
    protected override void OnInputFocusChanged(bool _focused)
    {
    }
}
