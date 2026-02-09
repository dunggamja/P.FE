using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;
using Battle;
using UnityEditor.Localization.Plugins.XLIFF.V20;


public class GUIElement_Dialogue : GUIElement
{
    [SerializeField]
    private RectTransform   m_root;

    [SerializeField]
    private TextMeshProUGUI m_name;

    [SerializeField]
    private TextMeshProUGUI m_text;

    [SerializeField]
    private ScrollRect      m_text_scroll;


    // private string m_text_name     = string.Empty;
    // private string m_text_dialogue = string.Empty;

    private int m_line_scroll_amount = 0;
    private int m_line_count      = 0;
    private int m_line_index      = 0;

    public void SetDialogue(DIALOGUE_PORTRAIT _portrait, string _dialogue)
    {
        OnClear();

        // m_text_name     = _name;
        // m_text_dialogue = _dialogue;

        SetActive(true);

        m_name.text = _portrait.Name;
        m_text.text = _dialogue;

        // 스크롤 위치 가장 맨위로.
        m_text_scroll.verticalNormalizedPosition = 1f;

        // 한번에 표시할 텍스트 줄 수 계산.
        (m_line_scroll_amount, m_line_count) = CalculateTextLineCount();
    }

    public void SetActive(bool _active)
    {
        m_root.gameObject.SetActive(_active);
    }

    private (int scroll_amount, int total) CalculateTextLineCount()
    {
        // 텍스트 갱신.
        m_text.ForceMeshUpdate();

        // 스크롤 영역 가져오기.
        var content  = m_text_scroll.content;
        var viewport = m_text_scroll.viewport;
        if (content == null || viewport == null)
            return (0, 0);

        // 이미 컨텐츠가 모두 표시되고 있는지 확인.
        float content_height         = content.rect.height;
        float viewport_height        = viewport.rect.height;
        float viewport_content_fit   = Mathf.Clamp01(viewport_height / Mathf.Max(1f, content_height));
        if   (viewport_content_fit  >= 0.99f)
            return (0, 0);


        // 텍스트 높이 계산.
        int text_line_count   = 0;
        var text_line_height  = m_text.fontSize;
        var text_info         = m_text.textInfo;
        if (text_info != null && text_info.lineCount > 0 && text_info.lineInfo != null)
        {
            text_line_height = text_info.lineInfo[0].lineHeight;
            text_line_count  = text_info.lineCount;
        }

        if (text_line_height <= 0f)
            return (0, 0);


        // 뷰포트 내에 표시될 텍스트 줄 수 계산.
        int     scroll_amount = (int)(viewport_height / text_line_height);
        return (scroll_amount, text_line_count);
    }


    public bool HasNextDialogue()
    {
        return m_line_index + m_line_scroll_amount < m_line_count;
    }


    public bool ScrollToNextDialogue()
    {
        if (HasNextDialogue() == false)
            return false;

        // line index 증가.
        m_line_index += m_line_scroll_amount;
        
        // 스크롤 이동.
        m_text_scroll.verticalNormalizedPosition = ScrollToLine(m_line_index);
        return true;
    }

    private float ScrollToLine(int _line_index)
    {
        // 스크롤 영역 가져오기.
        var content  = m_text_scroll.content;
        var viewport = m_text_scroll.viewport;
        if (content == null || viewport == null)
            return 1f;

        // 이미 컨텐츠가 모두 표시되고 있는지 확인.
        float content_height  = content.rect.height;
        float viewport_height = viewport.rect.height;
        float scroll_height   = content_height - viewport_height;
        if (scroll_height <= 0f)
            return 1f;

         var text_info = m_text.textInfo;
         if (text_info == null || text_info.lineInfo == null || text_info.lineCount == 0)         
            return 1f;

         
        _line_index         = Math.Clamp(_line_index, 0, text_info.lineCount - 1);

        // var first_line_position    = text_info.lineInfo[0].ascender;
        // var relative_line_position = target_line_position - first_line_position;
        // var target_line_position   = text_info.lineInfo[_line_index].ascender;


        var target_position = Math.Abs(text_info.lineInfo[_line_index].ascender);
        var scroll_position = 1f - (target_position / scroll_height);

        return scroll_position;
    }




    
}
