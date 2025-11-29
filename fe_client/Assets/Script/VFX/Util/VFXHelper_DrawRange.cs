using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

namespace Battle.MoveRange
{
  public enum EnumDrawFlag
  {    
    MoveRange     = 1 << 0,  // 이동 범위 포함.
    AttackRange   = 1 << 1,  // 공격 범위
    WandRange     = 1 << 2,  // 지팡이 범위
    ExchangeRange = 1 << 3,  // 교환 범위
  }

  // 위치별로 체크하는 인터페이스, 1칸 이상 이동하는지 체크하고, 
  // 벽 넘어가지 않는지 체크하고,
  // public interface IDrawRangeData
  // {
  //   public int DrawFlag { get; }
    
  //   public HashSet<(int x, int y)> List_Move   { get; }
  //   public HashSet<(int x, int y)> List_Weapon { get; }
    
  // }
  
  public class AttackRangeVisitor : 
      PathAlgorithm.IFloodFillVisitor, 
      IPoolObject 
      // IDrawRangeData
  {
    public int                DrawFlag     { get; set; } = 0;
    public TerrainMap         TerrainMap   { get; set; }
    public IPathOwner         Visitor      { get; set; }
    public (int x, int y)     Position     { get; set; }
    public int                MoveDistance { get; set; }
    public bool               VisitOnlyEmptyCell  => true;
    public bool               IsStop()            => false;
    public Int64              VisitorID    { get; set; } = 0;

    public (int min, int max)      WeaponRange   { get; set; } = (0, 0);
    public (int min, int max)      WandRange     { get; set; } = (0, 0);
    public (int min, int max)      ExchangeRange { get; set; } = (0, 0);

    public HashSet<(int x, int y)> Visit_Move     { get; private set; } = new();
    public HashSet<(int x, int y)> Visit_Weapon   { get; private set; } = new();
    public HashSet<(int x, int y)> Visit_Wand     { get; private set; } = new();
    public HashSet<(int x, int y)> Visit_Exchange { get; private set; } = new();



    public void Visit(int _visit_x, int _visit_y)
    {
        // 이동 범위에 넣어두자.
        Visit_Move.Add((_visit_x, _visit_y));
        
        var weapon_range_min = WeaponRange.min;
        var weapon_range_max = WeaponRange.max;        

        var wand_range_min   = WandRange.min;
        var wand_range_max   = WandRange.max;

        var exchange_range_min = ExchangeRange.min;
        var exchange_range_max = ExchangeRange.max;

        // 요거 GC 날거 같이 생겼군. Min(param[])
        var min_range        = Mathf.Min(weapon_range_min, wand_range_min, exchange_range_min);
        var max_range        = Mathf.Max(weapon_range_max, wand_range_max, exchange_range_max);


        // if (0 < min_range && 0 < max_range)
        {
          // 범위 체크.
          for(int x = -max_range; x <= max_range; ++x)
          {
              for(int y = -max_range; y <= max_range; ++y)
              {
                  var pos_x = _visit_x + x;
                  var pos_y = _visit_y + y;

                  // 맵 바운더리 체크.
                  if (TerrainMap != null && TerrainMap.IsInBound(pos_x, pos_y) == false)
                      continue;

                  // 사거리 체크.
                  var distance = PathAlgorithm.Distance(_visit_x, _visit_y, pos_x, pos_y);

                  // 공격 사거리.
                  if ((DrawFlag & (int)EnumDrawFlag.AttackRange) != 0)
                  {
                    if (weapon_range_min <= distance && distance <= weapon_range_max)
                        Visit_Weapon.Add((pos_x, pos_y));
                  }

                  // 지팡이 사거리.
                  if ((DrawFlag & (int)EnumDrawFlag.WandRange) != 0)
                  {
                    if (wand_range_min <= distance && distance <= wand_range_max)
                        Visit_Wand.Add((pos_x, pos_y));
                  }

                  // 교환 사거리.
                  if ((DrawFlag & (int)EnumDrawFlag.ExchangeRange) != 0)
                  {
                    if (exchange_range_min <= distance && distance <= exchange_range_max)
                        Visit_Exchange.Add((pos_x, pos_y));
                  }
              }
          }
        }

    }


    public AttackRangeVisitor SetData(
      int          _draw_flag,
      TerrainMap   _terrain, 
      Entity       _entity_object,
      bool         _use_base_position,
      Int64        _use_weapon_id)
    {
        DrawFlag     = _draw_flag;
        TerrainMap   = _terrain;

        if (_entity_object != null)
        {
            Visitor    = _entity_object;
            VisitorID    = _entity_object.ID;
            Position     = (_use_base_position) 
                          ? _entity_object.PathBasePosition 
                          : _entity_object.Cell;

            if ((DrawFlag & (int)EnumDrawFlag.MoveRange) != 0)
            {
                MoveDistance = _entity_object.PathMoveRange;
            }

            if ((DrawFlag & (int)EnumDrawFlag.AttackRange) != 0)
            {
                WeaponRange  = _entity_object.GetWeaponRange(_use_weapon_id);
            }

            if ((DrawFlag & (int)EnumDrawFlag.WandRange) != 0)
            {
                WandRange = _entity_object.GetWandRange(_use_weapon_id);
            }

            if ((DrawFlag & (int)EnumDrawFlag.ExchangeRange) != 0)
            {
                ExchangeRange = _entity_object.GetExchangeRange();
            }
        }

        return this;
    }

    public void Reset()
    {
        TerrainMap     = null;
        Visitor        = null;
        Position       = default;
        MoveDistance   = 0;
        VisitorID      = 0;
        WeaponRange    = (0, 0);
        WandRange      = (0, 0);
        ExchangeRange  = (0, 0);
        Visit_Move.Clear();
        Visit_Weapon.Clear();
        Visit_Wand.Clear();
        Visit_Exchange.Clear();
    }

  };



  public class VFXHelper_DrawRange
  {

    public int           DrawFlag           { get; private set; } = 0;
    public Int64         DrawEntityID       { get; private set; } = 0;
    public bool          UseBasePosition    { get; private set; } = false;
    public Int64         UseWeaponID        { get; private set; } = 0;
    public List<Int64>   VFXList            { get; private set; } = new List<Int64>();
    AttackRangeVisitor   AttackRangeVisitor { get; set; }         = new();


    public void DrawRange(int   _draw_flag
                        , Int64 _entityID
                        , bool  _use_base_position
                        , Int64 _use_weapon_id = 0)
    {
      // 파라미터 초기화.
      if (DrawFlag        == _draw_flag 
      &&  DrawEntityID    == _entityID
      &&  UseBasePosition == _use_base_position
      &&  UseWeaponID     == _use_weapon_id)
        return;

      Clear();

      // 파라미터 초기화.  
      if (_draw_flag == 0 || _entityID == 0)
        return;

      DrawFlag        = _draw_flag;
      DrawEntityID    = _entityID;
      UseBasePosition = _use_base_position;
      UseWeaponID     = _use_weapon_id;

      var terrain_map   = TerrainMapManager.Instance.TerrainMap;
      var entity_object = EntityManager.Instance.GetEntity(DrawEntityID);

      // 범위 체크.
      PathAlgorithm.FloodFill(
        AttackRangeVisitor.SetData(
          _draw_flag, 
          terrain_map, 
          entity_object, 
          UseBasePosition, 
          UseWeaponID));

      using var list_draw = HashSetPool<(int x, int y)>.AcquireWrapper();


      // 이동 범위 표시.
      foreach(var pos in AttackRangeVisitor.Visit_Move)
      {
        var vfx_id = VFXManager.Instance.CreateVFXAsync(
            ObjectPool<VFXShape.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_EFFECT_BLUE)
            .SetPosition(pos.CellToPosition())
            .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE)            
        );

        VFXList.Add(vfx_id);
        list_draw.Value.Add(pos);
      }

      // 공격 범위 표시.
      foreach(var pos in AttackRangeVisitor.Visit_Weapon)
      {
        // 이미 표시한 위치면 제외.
        if (list_draw.Value.Contains(pos))
          continue;

        var vfx_id = VFXManager.Instance.CreateVFXAsync(
            ObjectPool<VFXShape.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_EFFECT_RED)
            .SetPosition(pos.CellToPosition())
            .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE)
        );

        VFXList.Add(vfx_id);
        list_draw.Value.Add(pos);
      }   

      // 지팡이 범위 표시.
      foreach(var pos in AttackRangeVisitor.Visit_Wand)
      {
        // 이미 표시한 위치면 제외.
        if (list_draw.Value.Contains(pos))
          continue;

        var vfx_id = VFXManager.Instance.CreateVFXAsync(
            ObjectPool<VFXShape.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_EFFECT_GREEN)
            .SetPosition(pos.CellToPosition())
            .SetSnapToTerrain(true, Constants.BATTLE_VFX_SNAP_OFFSET_TILE)
        );

        VFXList.Add(vfx_id);
        list_draw.Value.Add(pos);
      }  


    }

    public void Clear()
    {
      DrawFlag     = 0;
      DrawEntityID = 0;

      AttackRangeVisitor.Reset();
      ReleaseVFX();
    }

    void ReleaseVFX()
    {
      foreach (var vfx in VFXList)
      {
        VFXManager.Instance.ReserveReleaseVFX(vfx);
      }

      VFXList.Clear();
    }

  }
}