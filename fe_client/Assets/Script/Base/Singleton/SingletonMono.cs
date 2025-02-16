using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
                    m_instance = FindFirstObjectByType<T>();

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

    public static void DestroySingleton()
    {        
        if (m_instance)
        {
            GameObject.Destroy(m_instance.gameObject);
            m_instance = null;
        }
    }

    private   float                 MIN_UPDATE_INTERVAL = 1f/120f;
    protected virtual float         LoopInterval        => 0f;
    private CancellationTokenSource LoopCancelToken     = null;

    
    protected virtual void Start()
    {
        OnInitialize();       
    } 

    private void OnApplicationQuit()
    {
        m_shutdown = true;
    }

    private void OnDestroy()
    {
        OnRelease(m_shutdown);
    }

    

    protected virtual void OnInitialize()
    {
        if (!this.name.Equals(Name))
             this.name = Name;

        CancelTask();
        
        LoopCancelToken = new CancellationTokenSource();
        StartLoop(LoopCancelToken.Token).Forget();
       
    }

    private async UniTask StartLoop(CancellationToken _token)
    {  
        try
        {
            while (true)
            {
                await UniTask.WaitForSeconds(Mathf.Max(MIN_UPDATE_INTERVAL, LoopInterval));

                _token.ThrowIfCancellationRequested();
                OnLoop();
            } 
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("Task was cancelled");
        }

    }

    protected virtual void OnLoop()
    {
       // Debug.LogWarning(this.GetType().Name);
    }

    protected virtual void OnRelease(bool _is_shutdown)
    { 
        if (!_is_shutdown)
        {
            CancelTask();
        }
    }

    private void CancelTask()
    {
        if (LoopCancelToken != null)
        {
            try
            {
                LoopCancelToken.Cancel();
                LoopCancelToken.Dispose();
            }
            catch (ObjectDisposedException)
            {
                Debug.LogWarning("LoopCancelToken already disposed");
            }
            finally
            {
                LoopCancelToken = null;
            }
        }
    }
}
