using System;
using System.Collections.Generic;
using UnityEngine;


public class SnapshotManager : Singleton<SnapshotManager>
{
    private Dictionary<Int64, GameSnapshot> m_snapshots = new();
    private Stack<Int64> m_snapshot_stack = new();
    
    // public Int64 CreateSnapshot(bool _is_full_snapshot)
    // {
    //     var snapshot_id = Util.GenerateID();
    //     var snapshot    = new GameSnapshot(snapshot_id, _is_full_snapshot);
        
    //     // ���� ���� ���¸� �������� ����
    //     // SaveCurrentState(snapshot);
        
    //     m_snapshots[snapshot_id] = snapshot;
    //     m_snapshot_stack.Push(snapshot_id);
        
    //     return snapshot_id;
    // }
    
    public void RestoreSnapshot(Int64 snapshot_id)
    {
        if (!m_snapshots.TryGetValue(snapshot_id, out var snapshot))
            return;
            
        // ������ ���·� ����
        // RestoreState(snapshot);
    }

    public GameSnapshot PeekSnapshot()
    {
        if (m_snapshot_stack.Count == 0)
            return null;

        var snapshot_id = m_snapshot_stack.Peek();

        if (!m_snapshots.TryGetValue(snapshot_id, out var snapshot))
            return null;

        return snapshot;
    }
    
    public void ClearSnapshots()
    {
        m_snapshots.Clear();
        m_snapshot_stack.Clear();
    }
}