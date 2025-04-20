using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;




public class AsyncOperationTracker
{
    private readonly Dictionary<Int64, CancellationTokenSource> _operationTokens = new();

    // ��Ƽ ������ ȯ���� �ƴ϶� �ּ�ó��.
    // private readonly CancellationTokenSourcePool _ctsPool = new();
    // private readonly object _lock = new object();

    public bool TryCancelOperation(Int64 id)
    {
        // lock (_lock)
        {
            if (_operationTokens.TryGetValue(id, out var cts))
            {
                _operationTokens.Remove(id);
                cts.Cancel();
                cts.Dispose();
                return true;
            }
            return false;
        }
    }

    public void TrackOperation(Int64 id, CancellationTokenSource cts)
    {
        // lock (_lock)
        {
            _operationTokens[id] = cts;
        }
    }

    public void CompleteOperation(Int64 id)
    {
        // lock (_lock)
        {
            if (_operationTokens.TryGetValue(id, out var cts))
            {
                _operationTokens.Remove(id);
                cts.Dispose();
            }
        }
    }



    public void Clear()
    {
        // lock (_lock)
        {
            _operationTokens.Clear();
        }
    }
}
