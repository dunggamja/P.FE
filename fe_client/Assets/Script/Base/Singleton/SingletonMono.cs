using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
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

    protected virtual void Start()
    {
        if (!this.name.Equals(Name))
             this.name = Name;
    } 

    private void OnApplicationQuit()
    {
        m_shutdown = true;
    }

    private void OnDestroy()
    {
        m_shutdown = true;
    }

}
