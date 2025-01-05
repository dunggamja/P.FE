using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


[EventReceiverAttribute(typeof(WorldObjectPositionEvent))]
public partial class WorldObjectManager 
{
    public void OnReceiveEvent(IEventParam _event)
    {

        if (_event is WorldObjectPositionEvent position_event)
        {
            OnReceiveEvent_WorldPositionEvent(position_event);
        }
        
    }

    void OnReceiveEvent_WorldPositionEvent(WorldObjectPositionEvent _event)
    {
        if (_event == null)
            return;

        var world_object = Seek(_event.ID);
        if (world_object == null)
            return;

        world_object.SetPositionData(_event.Position, _event.Position_Prev, Time.time);

    }

}