using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public partial class Entity
  {
    enum EnumEntityHUD
    {
      None,

      HP,
    }

    void CreateHUD_HP()
    {
      // HUD 생성.
      if (m_hud_list.TryGetValue(EnumEntityHUD.HP, out var hud_id))
      {
        GUIManager.Instance.CloseUI(hud_id);
        m_hud_list.Remove(EnumEntityHUD.HP);
      }
      
      // HUD 생성.
      var gui_id = GUIManager.Instance.OpenUI(
        HUD_Unit_HP.PARAM.Create(
          ID, 
          StatusManager.Status.GetPoint(EnumUnitPoint.HP)
          ));

      if (gui_id > 0)
      {
        m_hud_list.Add(EnumEntityHUD.HP, gui_id);
      }
    }

  }
}