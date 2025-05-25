using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EventDispatchManager : SingletonMono<EventDispatchManager>
{
    // HashSet<IEventReceiver>                       m_receivers          = new();
    Dictionary<System.Type, List<IEventReceiver>> m_receivers_by_type  = new(20);
    List<IEventParam>                             m_update_event_queue = new(10);
    List<IEventParam>                             m_dispatched_list    = new(10);
    
    
    Dictionary<Type, EventReceiverAttribute[]>    m_cached_attribute   = new(20);

    // List<IEventParam>                             m_cached_event_queue = new();


    EventReceiverAttribute[] TryGetAttribute(Type _type)
    {
        if (!m_cached_attribute.TryGetValue(_type, out var _attributes))
        {
            // EventReceiverAttributes만 선택해서 배열로 만들어줍니다. 
            _attributes = System.Attribute.GetCustomAttributes(_type)
            .Where(e => e is EventReceiverAttribute)
            .Select(e => e as EventReceiverAttribute)
            .ToArray();
             

            m_cached_attribute.Add(_type, _attributes);
        }

        return _attributes;
    }

    public void Reset()
    {
        // m_receivers.Clear();
        m_receivers_by_type.Clear();
        m_update_event_queue.Clear();
        m_dispatched_list.Clear();

        m_cached_attribute.Clear();
        // m_cached_event_queue.Clear();
    }

    public void AttachReceiver(IEventReceiver _receiver)
    {
        if (_receiver == null)
            return;

        var receiver_type = _receiver.GetType();
        var attributes    = TryGetAttribute(receiver_type);
        if (attributes   == null)
            return;

        // m_receivers.Add(_receiver);

        foreach(var e in attributes)
        {
            if (e.GetReceiveEventTypes() == null)
                continue;

            foreach(var event_type in e.GetReceiveEventTypes())
            {
                if (!m_receivers_by_type.TryGetValue(event_type, out var list_receiver))
                {
                    list_receiver = new List<IEventReceiver>();
                    m_receivers_by_type.Add(event_type, list_receiver);
                }

                list_receiver.Add(_receiver);
            }
        }
    }

    public void DetachReceiver(IEventReceiver _receiver)
    {
        if (_receiver == null)
            return;

        var receiver_type = _receiver.GetType();
        var attributes    = TryGetAttribute(receiver_type);
        if (attributes == null)
            return;

        // m_receivers.Remove(_receiver);

        // 
        foreach(var e in attributes)
        {
            var receiver_types = e.GetReceiveEventTypes();
            if (receiver_types == null || receiver_types.Length == 0)
                continue;

            foreach(var event_type in receiver_types)
            {
                if (m_receivers_by_type.TryGetValue(event_type, out var list_receiver))
                {
                    list_receiver.Remove(_receiver);
                }
            }
        }
    }


    public void UpdateEvent(IEventParam _event)
    {
        if (_event == null)
            return;

        switch (_event.EventProcessTiming)
        {
            // 즉시 처리.
            case EnumEventProcessTiming.Immediate:    DispatchEvent(_event);    break;
            
            // 다음 프레임에 처리.
            case EnumEventProcessTiming.OnNextUpdate: UpdateEventQueue(_event); break;
        }
        
    }

    // 이벤트 dispatch
    private void DispatchEvent(IEventParam _event)
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

        // 디스패치 완료 된 이벤트
        m_dispatched_list.Add(_event);

        //_event.Release();
        //ObjectPool.Release()
    }


    // Queue에 넣어놓고 Update에서 처리.
    private void UpdateEventQueue(IEventParam _event)
    {
        if (_event == null)
            return;

        m_update_event_queue.Add(_event);
    }


    private void DispatchEventQueue()
    {
        // 큐를 복사합시다.
        var list_event = ListPool<IEventParam>.Acquire();

        list_event.AddRange(m_update_event_queue);

        // 큐 클리어.
        m_update_event_queue.Clear();

        // 이벤트 디스패치.
        foreach (var e in list_event)
        {
            DispatchEvent(e);
        }

        // 
        ListPool<IEventParam>.Return(list_event);
    }

    private void PostDispatchedEvent()
    {
        foreach(var e in m_dispatched_list)
        {
            // Release 처리.
            if (e != null)
                e.Release();
        }

        m_dispatched_list.Clear();
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        // 이벤트 dispatch 처리.
        DispatchEventQueue();

        // 이벤트 dispatch 후처리.
        PostDispatchedEvent();
    }

}