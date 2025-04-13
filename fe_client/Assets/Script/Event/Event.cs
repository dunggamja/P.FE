using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumEventProcessTiming
{
    Immediate,    // 이벤트 Dispatch 시점에 즉시 처리.
    OnNextUpdate, // 다음 업데이트시 처리.
}

public interface IEventParam : IPoolObject
{
    EnumEventProcessTiming EventProcessTiming { get; }    

    void Release();
}



[System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)]
public class EventReceiverAttribute : System.Attribute
{
    private System.Type[] m_receive_event_types = null;

    public EventReceiverAttribute(params System.Type[] _receive_event_types)
    {
        m_receive_event_types = _receive_event_types;
    }

    public System.Type[] GetReceiveEventTypes() => m_receive_event_types;

}


public interface IEventReceiver
{
    void OnReceiveEvent(IEventParam _event);
}


