using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EventDispatchManager : SingletonMono<EventDispatchManager>
{
    // HashSet<IEventReceiver>                       m_receivers          = new();
    Dictionary<System.Type, List<IEventReceiver>> m_receivers_by_type  = new();
    List<IEventParam>                             m_update_event_queue = new();
    
    
    Dictionary<Type, EventReceiverAttribute[]>    m_cached_attribute   = new();

    List<IEventParam>                             m_cached_event_queue = new();

    List<IEventParam>                             m_cached_dispatched  = new();

    EventReceiverAttribute[] TryGetAttribute(Type _type)
    {
        if (!m_cached_attribute.TryGetValue(_type, out var _attributes))
        {
            // EventReceiverAttributes�� �����ؼ� �迭�� ������ݴϴ�. 
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
        m_cached_attribute.Clear();
        m_cached_event_queue.Clear();
        m_cached_dispatched.Clear();
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
            // ��� ó��.
            case EnumEventProcessTiming.Immediate:    DispatchEvent(_event);    break;
            
            // ���� �����ӿ� ó��.
            case EnumEventProcessTiming.OnNextUpdate: UpdateEventQueue(_event); break;
        }
        
    }

    // �̺�Ʈ dispatch
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


        m_cached_dispatched.Add(_event);

        //_event.Release();
        //ObjectPool.Release()
    }


    // Queue�� �־���� Update���� ó��.
    private void UpdateEventQueue(IEventParam _event)
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


    private void DispatchEventQueue()
    {
        // ť�� �����ѵڿ� ó���սô�. 
        m_cached_event_queue.AddRange(m_update_event_queue);

        // ť Ŭ����.
        m_update_event_queue.Clear();

        foreach (var e in m_cached_event_queue)
        {
            DispatchEvent(e);
        }

        // ��� �Ϸ������� Ŭ����.
        m_cached_event_queue.Clear();
    }

    private void PostDispatchedEvent()
    {
        foreach(var e in m_cached_dispatched)
        {
            // Release ó��.
            if (e != null)
                e.Release();
        }

        m_cached_dispatched.Clear();
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        // �̺�Ʈ dispatch ó��.
        DispatchEventQueue();

        // �̺�Ʈ dispatch ��ó��.
        PostDispatchedEvent();
    }

}