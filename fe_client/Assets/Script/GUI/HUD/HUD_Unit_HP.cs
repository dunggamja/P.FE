using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Battle;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(Battle_Scene_ChangeEvent),
    typeof(Battle_Entity_HP_UpdateEvent)
)]
public class HUD_Unit_HP : HUDBase, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public override EnumGUIType GUIType => EnumGUIType.HUD;

        public Int64 EntityID { get; private set; }
        public int  HP       { get; private set; }

        private PARAM(Int64 _entity_id, int _hp) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/hud/unit_hp",

            // is input enabled
            false)
        {
            EntityID = _entity_id;
            HP       = _hp;
        }

        static public PARAM Create(Int64 _entity_id, int _hp)
        {
            return new PARAM(_entity_id, _hp);
        }

    }


    [SerializeField]
    private Slider m_slider_hp;

    private Int64 m_entity_id = 0;

    protected override void OnOpen(GUIOpenParam _param)
    {
        var param =_param as PARAM;
        if (param == null)
        {
            Debug.LogError("HUD_Unit_HP: OnOpen param is null");
            return;
        }

        m_entity_id = param.EntityID;

        SetFollowWorldObjectID(param.EntityID);      

        SetHP(m_entity_id, param.HP);
    }

    protected override void OnClose()
    {
        // throw new NotImplementedException();
    }

    protected override void OnPostProcess_Close()
    {
        // throw new NotImplementedException();
    }

    public void OnReceiveEvent(IEventParam _event)
    {
        if (_event == null)
            return;

        switch (_event)
        {
            case Battle_Scene_ChangeEvent battle_scene_change_event:
                onReceiveEvent_Battle_Scene_ChangeEvent(battle_scene_change_event);
                break;

            case Battle_Entity_HP_UpdateEvent battle_entity_hp_update_event:
                OnReceiveEvent_Battle_Entity_HP_UpdateEvent(battle_entity_hp_update_event);
                break;
        }
    }

    void onReceiveEvent_Battle_Scene_ChangeEvent(Battle_Scene_ChangeEvent _event)
    {
        if (_event.IsEnter)
            Hide();
        else
            Show();
    }

    void OnReceiveEvent_Battle_Entity_HP_UpdateEvent(Battle_Entity_HP_UpdateEvent _event)
    {
        if (_event.EntityID == m_entity_id)
            SetHP(_event.EntityID, _event.HP);
    }


    void SetHP(Int64 _entity_id, int _hp)
    {
        var entity = EntityManager.Instance.GetEntity(_entity_id);
        if (entity == null)
            return;

        var hp_max = entity.StatusManager.Status.GetPoint(EnumUnitPoint.HP_Max);
        hp_max     = Math.Max(1, hp_max);


        if (m_slider_hp)
        {
            m_slider_hp.value = Mathf.Clamp01((float)_hp / hp_max);
        }

    }
}