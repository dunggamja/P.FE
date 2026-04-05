using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class MonoUpdator : MonoBehaviour 
{
   
    private   float                 MIN_UPDATE_INTERVAL = 1f/120f;
    protected virtual float         LoopInterval        => 0f;
    private CancellationTokenSource LoopCancelToken     = null;

    private   bool                  m_shutdown          = false;

    
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
      //   if (!this.name.Equals(Name))
      //        this.name = Name;

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
