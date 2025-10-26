using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;

public class GUIElement_Attack_Preview_Sequence_Grid : GUIElement
{
  [SerializeField]
  TextMeshProUGUI m_text_damage_atatcker;

  [SerializeField]
  TextMeshProUGUI m_text_damage_defender;

  [SerializeField]
  Image           m_image_damage_direction;


    protected override void Clear()
    {
        m_text_damage_atatcker.text = string.Empty;
        m_text_damage_defender.text = string.Empty;
        m_image_damage_direction.rectTransform.localScale = Vector3.one;
    }

    public void Initialize(bool _is_attacker_turn, int _damage)
    {
        Clear();

        if (_is_attacker_turn)
        {
            // 데미지 표시
            m_text_damage_defender.text = _damage.ToString();
        }
        else
        {
            // 데미지 표시
            m_text_damage_atatcker.text = _damage.ToString();
        }

        m_image_damage_direction.rectTransform.localScale 
          = (_is_attacker_turn) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
    }

}