using System.Collections.Generic;
using System.Threading;
using UnityEngine;




public class AsyncOperationTracker
{
    private readonly Dictionary<int, CancellationTokenSource> _operationTokens = new();
    // private readonly CancellationTokenSourcePool _ctsPool = new();
    // private readonly object _lock = new object();

    public bool TryCancelOperation(int id)
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

    public void TrackOperation(int id, CancellationTokenSource cts)
    {
        // lock (_lock)
        {
            _operationTokens[id] = cts;
        }
    }

    public void CompleteOperation(int id)
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
