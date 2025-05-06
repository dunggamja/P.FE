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
    private RectTransform   m_slot_cursor;
  
    private int             m_index = 0;
    private IDisposable     m_selected_index_subscription;

    public void Initialize(int index, Subject<int> indexSubject)
    {
        m_index = index;
        
        // 이전 구독이 있다면 해제
        m_selected_index_subscription?.Dispose();
        
        // 새로운 구독
        m_selected_index_subscription = indexSubject.Subscribe(i => 
            {
                m_slot_cursor.gameObject.SetActive(i == m_index);
            });
    }

    public void SetText(string _text)
    {
        if (m_text == null)
        {
            Debug.LogError("GUIElement_Grid_Item_MenuText: m_text is null");
            return;
        }

        m_text.text = _text;
    }

    private void OnDestroy()
    {
        m_selected_index_subscription?.Dispose();
    }


}
