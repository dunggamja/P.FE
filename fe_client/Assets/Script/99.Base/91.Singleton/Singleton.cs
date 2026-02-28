using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

    private   float                   MIN_LOOP_INTERVAL = 1f/120f;
    protected virtual float           LoopInterval      => 0f;
    private   CancellationTokenSource LoopCancelToken   = null;


 
    protected virtual void Init() 
    { 
        CancelTask();

        LoopCancelToken = new CancellationTokenSource();
        
        // 비동기 대기하지 않고 진행.
        StartLoop(LoopCancelToken.Token).Forget();
    }


    private async UniTask StartLoop(CancellationToken _token)
    {
        try
        {
            while (true)
            {
                await UniTask.WaitForSeconds(Mathf.Max(MIN_LOOP_INTERVAL, LoopInterval));
                _token.ThrowIfCancellationRequested();
                OnLoop();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task was cancelled");
        }

    }


    protected virtual void OnLoop()
    {  
        // Debug.LogWarning(this.GetType().Name);

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