using System;
using System.Collections.Generic;
using Battle;
using R3;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent)
    )]
public class GUIPage_Unit_Command_Attack_Preview : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }
        public Int64 TargetID { get; private set; }
        private PARAM(Int64 _entity_id, Int64 _target_id) 
        : base(
            // id      
            GUIPage.GenerateID(),           

            // asset path
            "gui/page/unit_command_attack_preview", 

            // gui type
            EnumGUIType.Screen,

            // is input enabled
            true
            )               
        {
            EntityID = _entity_id;
            TargetID = _target_id;
        }

        static public PARAM Create(Int64 _entity_id, Int64 _target_id)
        {
            return new PARAM(_entity_id, _target_id);
        }
    }

    private Int64 m_entity_id = 0;     
    private Int64 m_target_id = 0;

    public void OnReceiveEvent(IEventParam _event)
    {
        // throw new NotImplementedException();
    }

    protected override void OnOpen(GUIOpenParam _param)
    {
        // throw new NotImplementedException();
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;
        m_target_id = param?.TargetID ?? 0;

        // CombatSystemManager.Instance.Setup()

    }

    protected override void OnClose()
    {
        // throw new NotImplementedException();
    }

    protected override void OnPostProcess_Close()
    {
        // throw new NotImplementedException();
    }
}