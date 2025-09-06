using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;
using Battle;

public class GUIElement_Terrain_Attribute : GUIElement
{
    [SerializeField]
    private TextMeshProUGUI m_text_terrain_name;

    [SerializeField]
    private TextMeshProUGUI m_text_terrain_desc;


    private (int x, int y)  m_terrain_position = (0, 0);
    // private IDisposable     m_terrain_position_subscription;

    public void Initialize((int x, int y) _terrain_position)
    {
        Clear();

        m_terrain_position              = _terrain_position;
        // m_terrain_position_subscription = _subject_terrain_position.Subscribe(position => 
        // {
        //     m_terrain_position = position;
        //     UpdateText();
        // });

        UpdateText();
    }

    protected override void Clear()
    {
        // m_terrain_position_subscription?.Dispose();
        // m_terrain_position_subscription = null;

        m_text_terrain_name.text = string.Empty;
        m_text_terrain_desc.text = string.Empty;
    }


    void UpdateText()
    {
      // var terrain_map = TerrainMapManager.Instance.TerrainMap;
      // if (terrain_map == null)
      //   return;

      // var attribute = terrain_map.Attribute.GetAttribute(m_terrain_position.x, m_terrain_position.y);

      if (m_text_terrain_desc != null)
          m_text_terrain_desc.text = $"test.desc {m_terrain_position.x}, {m_terrain_position.y}";

      if (m_text_terrain_name != null)
          m_text_terrain_name.text = $"test.name {m_terrain_position.x}, {m_terrain_position.y}";
      
    }







}