using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;

/// <summary>이동 예정 경로 타일 VFX.</summary>
public class VFXHelper_Path
{
    public List<Int64>    VFXList   { get; private set; } = new ();

    public List<PathNode> PathNodes { get; private set; } = new ();

    public void Clear()
    {
        ReleaseTilePathVfx();
        PathNodes.Clear();
    }

    void ReleaseTilePathVfx()
    {
        foreach (var id in VFXList)
            VFXManager.Instance.ReserveReleaseVFX(id);

        VFXList.Clear();
    }


    bool IsEqualPathNodes(List<PathNode> _path_nodes)
    {
        var prev_count = PathNodes.Count;
        var new_count  = _path_nodes?.Count ?? 0;

        if (prev_count != new_count)
            return false;


        if (_path_nodes != null)
        {
            for (int i = 0; i < PathNodes.Count; i++)
                if (PathNodes[i].Equals(_path_nodes[i]) == false)
                    return false;
        }

        return true;
    }

    bool UpdatePathNodes(List<PathNode> _path_nodes)
    {
        if (IsEqualPathNodes(_path_nodes))
            return false;

        PathNodes.Clear();

        if (_path_nodes != null)
            PathNodes.AddRange(_path_nodes);

        return true;
    }



    public void DrawPath(List<PathNode> _path_nodes)
    {
        if (UpdatePathNodes(_path_nodes) == false)
            return;

        ReleaseTilePathVfx();

        foreach (var node in PathNodes)
        {
            // 유효한 위치가 아니면 제외.
            if (node.IsValidPosition() == false)
                continue;

            // 셀로 변환후 다시 포지션으로 변환.
            var cell_position = node.GetPosition().PositionToCell().CellToPosition();

            // VFX 생성.
            var vfx_id = VFXManager.Instance.CreateVFXAsync(
                ObjectPool<VFXShape.Param>.Acquire()
                    .SetVFXRoot_Default()
                    .SetVFXName(AssetName.TILE_EFFECT_PATH)
                    .SetPosition(cell_position)
                    .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE)
            );

            VFXList.Add(vfx_id);
        }
    }
}
