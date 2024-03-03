using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEventParam
{
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
    void OnReceiveEvent(IEventParam _param);
}


public class EventDispatchManager : Singleton<EventDispatchManager>
{
    HashSet<IEventReceiver>                       m_receivers         = new();
    Dictionary<System.Type, List<IEventReceiver>> m_receivers_by_type = new();
    

    protected override void Init()
    {
        base.Init();
    }

    public void Reset()
    {
        m_receivers.Clear();
        m_receivers_by_type.Clear();
    }

    public void AttachReceiver(IEventReceiver _receiver)
    {
        m_receivers.Add(_receiver);

        
        // 뭔가 복잡...
        foreach(var e in System.Attribute.GetCustomAttributes(_receiver.GetType()))
        {
            if (e is EventReceiverAttribute event_receiver_attribute)
            {
                if (event_receiver_attribute.GetReceiveEventTypes() != null)
                {
                    foreach(var event_type in event_receiver_attribute.GetReceiveEventTypes())
                    {
                        if (!m_receivers_by_type.TryGetValue(event_type, out var value))
                        {
                            value = new List<IEventReceiver>();
                            m_receivers_by_type.Add(event_type, value);
                        }

                        value.Add(_receiver);
                    }
                }
            }
        }
    }

    public void DetachReceiver(IEventReceiver _receiver)
    {
        m_receivers.Remove(_receiver);

        // 
        foreach(var e in System.Attribute.GetCustomAttributes(_receiver.GetType()))
        {
            var event_receiver_attribute = e as EventReceiverAttribute ;
            if (event_receiver_attribute == null)
                continue;
            
            var receiver_types = event_receiver_attribute.GetReceiveEventTypes();
            if (receiver_types == null || receiver_types.Length == 0)
                continue;

            foreach(var event_type in receiver_types)
            {
                if (m_receivers_by_type.TryGetValue(event_type, out var value))
                {
                    value.Remove(_receiver);
                }
            }
        }
    }

    public void DispatchEvent(IEventParam _param)
    {
        var event_type = _param.GetType();

        if (m_receivers_by_type.TryGetValue(event_type, out var receivers))
        {
            foreach (var e in receivers)
            {
                if (e != null)
                    e.OnReceiveEvent(_param);
            }
        }

    }

}