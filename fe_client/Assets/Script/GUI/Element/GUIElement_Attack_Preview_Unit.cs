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
  Slider                       m_slider_hp_before;
  [SerializeField]
  Slider                       m_slider_hp_after;

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

    var weapon              = entity.Inventory.GetItem(_weapon_id);
    var weapon_name_key     = weapon.GetLocalizeName();
    var weapon_name_subject = LocalizationManager.Instance.GetTextObservable(
            weapon_name_key.Table, 
            weapon_name_key.Key);

    // TODO: 엔티티 이름 필요.
    var entity_name_key     = LocalizeKey.Create("localization_base", "ui_empty");
    var entity_name_subject = LocalizationManager.Instance.GetTextObservable(
            entity_name_key.Table, 
            entity_name_key.Key);

    // 무기 이름
    m_text_weapon_name_subscription = weapon_name_subject
      .Subscribe(text => m_text_weapon_name.text = text);

    // 엔티티 이름
    m_text_unit_name_subscription = entity_name_subject
      .Subscribe(text => m_text_unit_name.text = text);

    // 데미지 텍스트
    m_text_damage.text    = _damage.ToString();
    m_text_critical.text  = $"{_critical} %";
    m_text_hit.text       = $"{_hit} %";
    m_text_hp_before.text = _hp_before.ToString();
    m_text_hp_after.text  = _hp_after.ToString();


    // HP 슬라이더
    var hp_max               = entity.StatusManager.Status.GetPoint(EnumUnitPoint.HP_Max);
    m_slider_hp_before.value = Mathf.Clamp01((float)_hp_before / Math.Max(1, hp_max));
    m_slider_hp_after.value  = Mathf.Clamp01((float)_hp_after  / Math.Max(1, hp_max));

    // 타일 속성
    m_terrain_attribute.Initialize(entity.Cell);

  }

  protected override void Clear()
  {
    m_text_unit_name_subscription?.Dispose();
    m_text_weapon_name_subscription?.Dispose();

    m_text_unit_name_subscription   = null;
    m_text_weapon_name_subscription = null;
  }
}