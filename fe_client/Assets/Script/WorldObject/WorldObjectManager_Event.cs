using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


[EventReceiverAttribute(typeof(Battle.WorldPositionEvent))]
public partial class WorldObjectManager 
{
    public void OnReceiveEvent(IEventParam _event)
    {

        if (_event is Battle.WorldPositionEvent position_event)
        {
            
        }
        
    }

}