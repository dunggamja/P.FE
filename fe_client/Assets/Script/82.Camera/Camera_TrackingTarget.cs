using UnityEngine;
using Battle;
using R3;
using System;

[EventReceiver(typeof(Battle_Camera_PositionEvent))]
public class Camera_TrackingTarget : MonoBehaviour, IEventReceiver
{
    void OnEnable()
    {
        EventDispatchManager.Instance.AttachReceiver(this);        
    }

    void OnDisable()
    {
        try
        {
            EventDispatchManager.Instance.DetachReceiver(this);        
        }
        catch (Exception)
        {
            //Debug.LogError($"Camera_TrackingTarget, OnDisable, Exception:{_exception.Message}");
        }
    }

    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case Battle_Camera_PositionEvent _camera_position_event:
                OnReceiveEvent_CameraPositionEvent(_camera_position_event);
                break;
        }
        // throw new NotImplementedException();
    }

    void OnReceiveEvent_CameraPositionEvent(Battle_Camera_PositionEvent _event)
    {
        if (_event == null)
            return;

        var world_position = _event.Cell.CellToPosition();
        world_position.y   = TerrainMapManager.Instance.GetWorldHeight(world_position);

        transform.position = world_position;
    }
}
