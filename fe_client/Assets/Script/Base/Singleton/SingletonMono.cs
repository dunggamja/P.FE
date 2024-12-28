using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    static T    m_instance;
    static bool m_shutdown = false;
    //static Object m_lock;

    protected static string Name => "@Singleton_" + typeof(T).ToString();
    
    public static T Instance
    {
        get
        {
            if (m_shutdown)
            {
                return null;
            }

            //lock(m_lock)
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();

                    if (m_instance == null)
                    {
                        var new_object  = new GameObject();
                        new_object.name = Name;
                        m_instance      = new_object.AddComponent<T>();

                        DontDestroyOnLoad(new_object);
                    }
                }
            }

            return m_instance;
        }
    }


    
    private   float         MIN_UPDATE_INTERVAL = 1f/60f;
    protected virtual float UpdateInterval     => 0f;
    private   float         m_last_update_time =  0f;

    private   bool  IsUpdateTime()
    {
        var cur_time = Time.time;
        var interval = Mathf.Max(MIN_UPDATE_INTERVAL, UpdateInterval);

        if (cur_time - m_last_update_time >= interval)
        {
            m_last_update_time = cur_time;
            return true;
        }
        return false;
    }


    
    protected virtual void Start()
    {
        OnInitialize();       
    } 

    // Update is called once per frame
    private void Update()
    {
        if (IsUpdateTime())
        {
            OnUpdate();
        }
    }

    private void OnApplicationQuit()
    {
        m_shutdown = true;
    }

    private void OnDestroy()
    {
        m_shutdown = true;
    }

    

    protected virtual void OnInitialize()
    {
        if (!this.name.Equals(Name))
             this.name = Name;
    }

    protected virtual void OnUpdate()
    { }
}
