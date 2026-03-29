using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent),
    typeof(GUI_Menu_CancelEvent),
    typeof(Battle_Scene_ChangeEvent),
    typeof(Battle_Command_Event)
    )]
public class GUIPage_Unit_Select_Target : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        public Int64                EntityID       { get; private set; } = 0;
        public List<Int64>          TargetList     { get; private set; } = new();
        public Action<Int64>        OnSelectTarget { get; private set; } = null;

        private PARAM(Int64 _entity_id, List<Int64> _target_list, Action<Int64> _on_select_target) 
        : base(
            // id      
            GUIPage.GenerateID(),           
            // asset path
            "gui/page/unit_select_target", 

            // is input enabled
            true,

            // is multiple open
            false
        )
        {
            EntityID       = _entity_id;
            TargetList.AddRange(_target_list);
            OnSelectTarget = _on_select_target;
        }

        static public PARAM Create(Int64 _entity_id, List<Int64> _target_list, Action<Int64> _on_select_target)
        {
            return new PARAM(_entity_id, _target_list, _on_select_target);
        }
    }


    private Int64                m_entity_id    = 0;
    private List<Int64>          m_target_list  = new();
    private int                  m_target_index = 0;
    private Int64                m_vfx_target   = 0;

    private Action<Int64>        m_on_select_target = null;



    private Int64 TargetID
    {
        get
        {
            if (m_target_index < 0 || m_target_index >= m_target_list.Count)
                return 0;
            return m_target_list[m_target_index];
        }
    }


    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case Battle_Scene_ChangeEvent:
                CloseSelf();
                break;

            case GUI_Menu_MoveEvent menu_move_event:
                OnReceiveEvent_GUI_Menu_MoveEvent(menu_move_event);
                break;

            case GUI_Menu_SelectEvent menu_select_event:
                OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
                break;

            case GUI_Menu_CancelEvent menu_cancel_event:
                OnReceiveEvent_GUI_Menu_CancelEvent(menu_cancel_event);
                break;

            case Battle_Command_Event battle_command_event:
                OnReceiveEvent_Battle_CommandEvent(battle_command_event);
                break;
        }
    }    


    protected override void OnOpen(GUIOpenParam _param)
    {
        var param          = _param as PARAM;
        m_entity_id        = param.EntityID;
        m_target_list.AddRange(param.TargetList);
        m_target_index     = 0;
        m_on_select_target = param.OnSelectTarget;

        CreateTargetVFX();
        UpdateTargetVFX();
    }

    protected override void OnClose()
    {
        ReleaseTargetVFX();
    }

    protected override void OnPostProcess_Close()
    {
    }



    private void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {      
        if (_event == null || _event.GUI_ID != ID)
            return;


        m_on_select_target?.Invoke(TargetID);
    }

    private void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
         if (_event == null || _event.GUI_ID != ID)
               return;

         // 메뉴 이동 방향이 없으면 종료.
         var move_direction = _event.MoveDirection;
         if (move_direction.y  == 0 && move_direction.x == 0)
            return;

         // 타겟 리스트가 없으면 종료.
         if (m_target_list.Count == 0)
            return;

         var move_x       = Math.Clamp(_event.MoveDirection.x, -1, 1);
         var move_y       = Math.Clamp(_event.MoveDirection.y, -1, 1);

         // 타겟 이동 방향에 따라 인덱스 추가.
         int offset =
            (move_x != 0) 
          ? (move_x > 0 ? 1 : -1) 
          : (move_y != 0) 
          ? (move_y > 0 ? 1 : -1) 
          : 0;

         // 타겟 변경.
         var prev_target_id  = TargetID;
         m_target_index     += offset;
         m_target_index     += m_target_list.Count;
         m_target_index     %= m_target_list.Count;
         var new_target_id   = TargetID;

         // 타겟 변경 처리.
         if (prev_target_id != new_target_id)
         {
            UpdateTargetVFX();
         }
    }

    private void OnReceiveEvent_GUI_Menu_CancelEvent(GUI_Menu_CancelEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        CloseSelf();
    }



    void OnReceiveEvent_Battle_CommandEvent(Battle_Command_Event _event)
    {
        if (_event == null || _event.EntityID != m_entity_id)
            return;

        CloseSelf();
    }
    void CreateTargetVFX()
    {
        var entity = EntityManager.Instance.GetEntity(TargetID);
        if (entity == null)
            return;

        m_vfx_target = VFXHelper.CreateTileSelctVFX(entity.Cell);
    }

    void UpdateTargetVFX()
    {
        var entity = EntityManager.Instance.GetEntity(TargetID);
        if (entity == null)
            return;

        VFXHelper.UpdateTileSelectVFX(m_vfx_target, entity.Cell);
    }

    void ReleaseTargetVFX()
    {
        VFXHelper.ReleaseTileSelectVFX(ref m_vfx_target);
    }


}