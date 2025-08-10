using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;

public class GUIElement_Grid_Item_MenuText : GUIElement
{


    [SerializeField]
    private TextMeshProUGUI m_text;

    [SerializeField]
    private RectTransform   m_cursor;
  
    private int             m_index = 0;
    private IDisposable     m_selected_index_subscription;
    private IDisposable     m_text_subscription;
    public void Initialize(int _index,
        Observable<int>        _subject_select_index,
        Observable<string>     _subject_text)
    {
        Clear();
        
        m_index = _index;

        m_cursor.gameObject.SetActive(false);        
        
        // 새로운 구독.
        m_selected_index_subscription = _subject_select_index.Subscribe(i => 
            {
                m_cursor.gameObject.SetActive(i == m_index);
            });

        // 텍스트 구독.
        m_text_subscription = _subject_text.Subscribe(text =>
        {
            m_text.text = text;
        });

        gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        // TODO: 이것은 임시 코드임....
        Clear();
    }


    protected override void Clear()
    {
        m_selected_index_subscription?.Dispose();
        m_text_subscription?.Dispose();

        m_selected_index_subscription = null;
        m_text_subscription           = null;
        m_index                       = 0;
    }


}
