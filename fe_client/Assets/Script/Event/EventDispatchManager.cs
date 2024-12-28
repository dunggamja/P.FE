using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventDispatchManager : SingletonMono<EventDispatchManager>
{
    HashSet<IEventReceiver>                       m_receivers          = new();
    Dictionary<System.Type, List<IEventReceiver>> m_receivers_by_type  = new();
    List<IEventParam>                             m_update_event_queue = new();


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

    // 이벤트 즉시 처리.
    public void DispatchEvent(IEventParam _event)
    {
        if (_event == null)
            return;

        var event_type = _event.GetType();
        if (m_receivers_by_type.TryGetValue(event_type, out var receivers))
        {
            foreach (var e in receivers)
            {
                if (e != null)
                    e.OnReceiveEvent(_event);
            }
        }
    }

    // Queue에 넣어놓고 Update에서 처리.
    public void EnqueueEvent(IEventParam _event)
    {
        if (_event == null)
            return;

        m_update_event_queue.Add(_event);

        // var event_type = _event.GetType();

        // if (!m_update_event_queue.TryGetValue(event_type, out var value))
        // {
        //     value = new List<IEventParam>();
        //     m_update_event_queue.Add(event_type, value);
        // }

        // value.Add(_event);
    }


    private void DispatchUpdateEventQueue()
    {
        // 큐를 복사한뒤에 처리합시다. 
        var copy_queue = new List<IEventParam>(m_update_event_queue);
        // 큐 클리어.
        m_update_event_queue.Clear();

        foreach (var e in copy_queue)
        {
            DispatchEvent(e);
        }

        // // 타입별로 event들 dispatch
        // foreach((var event_type, var list_event) in copy_queue)
        // {
        //     if (m_receivers_by_type.TryGetValue(event_type, out var list_receiver))
        //     {
        //         foreach (var receiver in list_receiver)
        //         {
        //             foreach(var e in list_event)
        //                 receiver.OnReceiveEvent(e);
        //         }
        //     }
        // }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        DispatchUpdateEventQueue();
    }

}