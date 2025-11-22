using System;
using System.Collections.Generic;
using UnityEngine;
using Battle;
using R3;

[EventReceiver(
   typeof(Battle_Command_Event),
   typeof(Battle_Cursor_PositionEvent))]
public class CameraMananger : Singleton<CameraMananger>, IEventReceiver
{
   //  private (int x, int y) m_last_camera_position = (0, 0);
    private (int x, int y) m_last_cursor_position    = (0, 0);
    private Int64          m_last_tracking_entity_id = 0;

    protected override void Init()
    {
        base.Init();

        // event receiver 
        EventDispatchManager.Instance.AttachReceiver(this);
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        Update_CameraTrackingTarget();
    }

    void Update_CameraTrackingTarget()
    {
         var tracking_position = Collect_CameraTrackingTarget();
         if (tracking_position != (0, 0))
         {
            EventDispatchManager.Instance.UpdateEvent(
               ObjectPool<Battle_Camera_PositionEvent>.Acquire()
               .SetCell(tracking_position));
         }
    }


   (int x, int y) Collect_CameraTrackingTarget()
   {
      
      if (m_last_tracking_entity_id > 0)
      {
         var entity_id             = m_last_tracking_entity_id;
         m_last_tracking_entity_id = 0;
         
         // 타겟의 위치를 반환합니다.
         var entity = EntityManager.Instance.GetEntity(entity_id);
         if (entity != null)
            return entity.Cell;
      }


      if (m_last_cursor_position != (0, 0))
      {
         var cursor_position    = m_last_cursor_position;
         m_last_cursor_position = (0, 0);

         return cursor_position;
      }

      return (0, 0);
   }

    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case Battle_Command_Event _command_event:
                OnReceiveEvent_CommandEvent(_command_event);
                break;
            case Battle_Cursor_PositionEvent _cursor_position_event:
                OnReceiveEvent_CursorPositionEvent(_cursor_position_event);
                break;
        }
    }

    void OnReceiveEvent_CommandEvent(Battle_Command_Event _event)
    {
        if (_event == null)
            return;

        var command_type = _event.CommandType;
        switch (command_type)
        {
            // 카메라 추적을 진행하는 명령들.
            case EnumCommandType.Attack:
            case EnumCommandType.Move:
            case EnumCommandType.Wand:
               m_last_tracking_entity_id = _event.EntityID;
               break;
        }
    }

    void OnReceiveEvent_CursorPositionEvent(Battle_Cursor_PositionEvent _event)
    {
        if (_event == null)
            return;

        m_last_cursor_position = _event.Cell;
    }
}