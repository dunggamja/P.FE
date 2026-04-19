using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;

/// <summary>이동 예정 경로 타일 VFX.</summary>
public static class VFXHelper_Path
{
    public static void ReleaseTilePathVfx(ref List<Int64> _vfx_ids)
    {
        if (_vfx_ids == null || _vfx_ids.Count == 0)
            return;

        foreach (var id in _vfx_ids)
            VFXManager.Instance.ReserveReleaseVFX(id);

        _vfx_ids.Clear();
    }

    public static void ApplyTilePathVfx(ref List<Int64> _vfx_ids, List<PathNode> _path_nodes)
    {
        ReleaseTilePathVfx(ref _vfx_ids);
        if (_path_nodes == null || _path_nodes.Count == 0)
            return;

        if (_vfx_ids == null)
            _vfx_ids = new List<Int64>();

        (int x, int y)? prev = null;
        foreach (var node in _path_nodes)
        {
            if (node.IsValidPosition() == false)
                continue;

            var cell = node.GetPosition().PositionToCell();
            if (prev.HasValue && prev.Value.x == cell.x && prev.Value.y == cell.y)
                continue;

            prev = cell;

            var vfx_id = VFXManager.Instance.CreateVFXAsync(
                ObjectPool<VFXShape.Param>.Acquire()
                    .SetVFXRoot_Default()
                    .SetVFXName(AssetName.TILE_EFFECT_PATH)
                    .SetPosition(cell.CellToPosition())
                    .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE)
            );

            _vfx_ids.Add(vfx_id);
        }
    }
}
