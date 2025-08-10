using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.UI;
using Battle;


public class GUIElement_Attack_Preview_Unit : GUIElement
{
  [SerializeField]
  GUIElement_Terrain_Attribute m_terrain_attribute;

  [SerializeField]
  TextMeshProUGUI              m_text_unit_name;

  [SerializeField]
  TextMeshProUGUI              m_text_weapon_name;        

  [SerializeField]
  TextMeshProUGUI              m_text_damage;

  [SerializeField]
  TextMeshProUGUI              m_text_critical;

  [SerializeField]
  TextMeshProUGUI              m_text_hit;

  [SerializeField]
  Slider                       m_slider_hp;

  [SerializeField]
  TextMeshProUGUI              m_text_hp_before;

  [SerializeField]
  TextMeshProUGUI              m_text_hp_after;

  private IDisposable m_text_unit_name_subscription;
  private IDisposable m_text_weapon_name_subscription;

  public void Initialize(
    Int64 _entity_id,
    Int64 _weapon_id,
    Int32 _damage,
    Int32 _critical,
    Int32 _hit,
    Int32 _hp_before,
    Int32 _hp_after)
  {
    Clear();


    var entity = EntityManager.Instance.GetEntity(_entity_id);
    if (entity == null)
      return;

    var weapon       = entity.Inventory.GetItem(_weapon_id);
    var localize_key = weapon.GetLocalizeName();
    // m_text_weapon_name_subscription = LocalizationManager.Instance.GetTextObservable(
    //         localize_key.Table, 
    //         localize_key.Key)
    //         .Subscribe(text => m_text_weapon_name.text = text);



    // m_text_unit_name.text = $"이름:{entity.ID}"; // TODO: 이름 추가 필요.
    // m_text_weapon_name.text = ;

  }

  protected override void Clear()
  {
    m_text_unit_name_subscription?.Dispose();
    m_text_weapon_name_subscription?.Dispose();

    m_text_unit_name_subscription   = null;
    m_text_weapon_name_subscription = null;
  }
}