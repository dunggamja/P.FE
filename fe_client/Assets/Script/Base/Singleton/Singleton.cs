using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Singleton<T> where T : Singleton<T>, new()
{
    static T      m_instance;
    //static Object m_lock;
    
    public static T Instance
    {
        get
        {
            //lock(m_lock)
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                    m_instance.Init();
                }
            }

            return m_instance;
        }
    }

    protected virtual void Init() { }
}