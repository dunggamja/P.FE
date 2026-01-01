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


    [SerializeField]
    private RectTransform   m_select_rect;
  
    private int             m_index = 0;
    // private IDisposable     m_cursor_index_subscription;
    // private IDisposable     m_text_subscription;
    // private IDisposable     m_select_index_subscription;

    public void Initialize(
        int                    _index,
        Observable<int>        _subject_cursor_index = null,
        Observable<string>     _subject_text         = null,
        Observable<int>        _subject_select_index = null)
    {
        OnClear();
        
        m_index = _index;

        if (m_cursor)
        {
            m_cursor.gameObject.SetActive(false);        
            
            if (_subject_cursor_index != null)
            {
                _subject_cursor_index.Subscribe(i => 
                {
                    m_cursor.gameObject.SetActive(i == m_index);
                    }).AddTo(m_disposables);
            }
        }

        if (m_text)
        {
            m_text.gameObject.SetActive(false);

            if (_subject_text != null)
            {
                _subject_text.Subscribe(text =>
                {
                        m_text.gameObject.SetActive(true);
                        m_text.text = text;
                    }).AddTo(m_disposables);
            }
        }        

        if (m_select_rect)
        {
            m_select_rect.gameObject.SetActive(false);

            if (_subject_select_index != null)
            {
                _subject_select_index.Subscribe(i => 
                        {
                            m_select_rect.gameObject.SetActive(i == m_index);
                        }).AddTo(m_disposables);
            }

        }

        gameObject.SetActive(true);
    }

    protected override void OnClear()
    {
        base.OnClear();
        // m_cursor_index_subscription?.Dispose();
        // m_text_subscription?.Dispose();
        // m_select_index_subscription?.Dispose();

        // m_cursor_index_subscription = null;
        // m_text_subscription         = null;
        // m_select_index_subscription = null;
        m_index                     = 0;
    }


}
