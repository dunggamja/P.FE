using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


[EventReceiver(typeof(WorldObject_PositionEvent))]
public partial class WorldObjectManager 
{
    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case WorldObject_PositionEvent position_event:
                OnReceiveEvent_WorldPositionEvent(position_event);
                break;
        }
        
    }

    void OnReceiveEvent_WorldPositionEvent(WorldObject_PositionEvent _event)
    {
        if (_event == null)
            return;

        var world_object = Seek(_event.ID);
        if (world_object == null)
            return;

        // ���� ������Ʈ�� ũ�⸦ ����Ͽ� ��ġ ����
        var half_size = new Vector3(1f, 0f, 1f) * 0.5f;

        world_object.SetPositionData(
            _event.Position + half_size,
            _event.Position_Prev + half_size,
            Time.time);

    }

}