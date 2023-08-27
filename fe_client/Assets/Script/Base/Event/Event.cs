using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEventParam
{
}

public interface IEventReceiver
{
    void OnReceiveEvent(IEventParam _param);
}


public class EventManager : Singleton<EventManager>
{
    HashSet<IEventReceiver> m_receivers = new HashSet<IEventReceiver>();

    protected override void Init()
    {
        base.Init();
    }

    public void Reset()
    {
        m_receivers.Clear();
    }

    public void AttachReceiver(IEventReceiver _receiver)
    {
        m_receivers.Add(_receiver);
    }

    public void DetachReceiver(IEventReceiver _receiver)
    {
        m_receivers.Remove(_receiver);
    }

    public void DispatchEvent(IEventParam _param)
    {
        foreach(var e in m_receivers)
        {
            e.OnReceiveEvent(_param);
        }
    }

}