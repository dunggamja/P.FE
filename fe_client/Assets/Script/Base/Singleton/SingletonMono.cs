using System;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    static T    m_instance;
    static bool m_shutdown = false;
    //static Object m_lock;

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
                        new_object.name = "@Singleton_" + typeof(T).ToString();
                        m_instance      = new_object.AddComponent<T>();

                        DontDestroyOnLoad(new_object);
                    }
                }
            }

            return m_instance;
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

}
