using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


[EventReceiver(
    typeof(WorldObject_PositionEvent),
    typeof(WorldObject_ShowEvent))]
public partial class WorldObjectManager 
{
    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case WorldObject_PositionEvent position_event:
                OnReceiveEvent_WorldPositionEvent(position_event);
                break;
            case WorldObject_ShowEvent show_event:
                OnReceiveEvent_WorldShowEvent(show_event);
                break;
        }
        
    }

    private void OnReceiveEvent_WorldShowEvent(WorldObject_ShowEvent show_event)
    {
        if (show_event == null)
            return;

        var world_object = Seek(show_event.ID);
        if (world_object == null)
            return;

        world_object.SetShow(show_event.Show);
    }

    void OnReceiveEvent_WorldPositionEvent(WorldObject_PositionEvent _event)
    {
        if (_event == null)
            return;

        var world_object = Seek(_event.ID);
        if (world_object == null)
            return;

 

        world_object.SetPositionData(
            _event.Position,
            _event.Position_Prev,
            Time.time);

    }

}