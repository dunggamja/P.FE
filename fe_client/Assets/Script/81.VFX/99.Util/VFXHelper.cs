using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Battle;

public static class VFXHelper
{
    public static Int64 CreateTileSelctVFX((int x, int y) _cell)
    {
        // 커서 VFX 생성.
        var vfx_param = ObjectPool<VFXObject.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetPosition(_cell.CellToPosition())
            .SetVFXName(AssetName.TILE_SELECTION)
            .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE);

        return VFXManager.Instance.CreateVFXAsync(vfx_param);
    }

    public static void ReleaseTileSelectVFX(ref Int64 _vfx_id)
    {
        VFXManager.Instance.ReserveReleaseVFX(_vfx_id);
        _vfx_id = 0;
    }

    public static void UpdateTileSelectVFX(Int64 _vfx_id, (int x, int y) _cell)
    {
        // 커서 VFX 위치 갱신.
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<VFX_TransformEvent>.Acquire()
            .SetID(_vfx_id)
            .SetPosition(_cell.CellToPosition())                
        ); 

        // 커서 위치 이벤트 발생. (카메라 갱신)
        EventDispatchManager.Instance.UpdateEvent(
            ObjectPool<Battle_Cursor_PositionEvent>.Acquire()
            .Set(_cell)
        ); 
    }

}
