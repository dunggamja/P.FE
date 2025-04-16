using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIElement_Grid_Item_MenuText : GUIElement
{
    [SerializeField]
    private TextMeshProUGUI m_text;

    public void SetText(string _text)
    {
        if (m_text == null)
        {
            Debug.LogError("GUIElement_Grid_Item_MenuText: m_text is null");
            return;
        }

        m_text.text = _text;
    }
}
