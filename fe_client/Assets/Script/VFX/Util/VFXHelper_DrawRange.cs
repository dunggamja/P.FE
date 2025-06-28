using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

namespace Battle.MoveRange
{
  public enum EnumDrawFlag
  {    
    MoveRange   = 1 << 0,    // 이동 거리
    AttackRange = 1 << 1,  // 무기 범위
  }
  
  class MoveRangeVisitor : PathAlgorithm.IFloodFillVisitor
  {
      public int                DrawFlag     { get; set; } = 0;
      public TerrainMap         TerrainMap   { get; set; }
      public IPathOwner         Visitor      { get; set; }
      public (int x, int y)     Position     { get; set; }
      public int                MoveDistance { get; set; }
      public Int64              VisitorID    { get; set; } = 0;
      public (int min, int max) WeaponRange  { get; set; } = (0, 0);

      public HashSet<(int x, int y)> List_Move   { get; set; } = new();
      public HashSet<(int x, int y)> List_Weapon { get; set; } = new();

      public void Visit(int _visit_x, int _visit_y)
      {
          List_Move.Add((_visit_x, _visit_y));

          
          var weapon_range_min = WeaponRange.min;
          var weapon_range_max = WeaponRange.max;

          

          // 무기 사거리 범위.
          for(int x = -weapon_range_max; x <= weapon_range_max; ++x)
          {
              for(int y = -weapon_range_max; y <= weapon_range_max; ++y)
              {
                  var weapon_x = _visit_x + x;
                  var weapon_y = _visit_y + y;

                  // 무기 사거리 체크
                  var distance = PathAlgorithm.Distance(_visit_x, _visit_y, weapon_x, weapon_y);
                  if (distance < weapon_range_min || weapon_range_max < distance)
                  {
                      continue;
                  }

                  // 맵 범위 체크.
                  if (TerrainMap != null)
                  {
                      if (weapon_x < 0 || weapon_y < 0)
                          continue;

                      if (TerrainMap.Height <= weapon_y || TerrainMap.Width <= weapon_x)
                          continue;
                  }

                  List_Weapon.Add((weapon_x, weapon_y));
              }
          }
      }


      public MoveRangeVisitor SetData(
        int          _draw_flag,
        TerrainMap   _terrain, 
        Entity       _entity_object)
      {
          DrawFlag     = _draw_flag;
          TerrainMap   = _terrain;

          if (_entity_object != null)
          {
              Visitor    = _entity_object;
              VisitorID    = _entity_object.ID;
              Position     = _entity_object.PathBasePosition;

              if ((DrawFlag & (int)EnumDrawFlag.MoveRange) != 0)
              {
                MoveDistance = _entity_object.PathMoveRange;
              }

              if ((DrawFlag & (int)EnumDrawFlag.AttackRange) != 0)
              {
                WeaponRange  = _entity_object.GetWeaponRange();
              }
          }

          return this;
      }

      public MoveRangeVisitor Reset()
      {
          TerrainMap     = null;
          Visitor      = null;
          Position       = default;
          MoveDistance   = 0;
          VisitorID      = 0;
          WeaponRange    = (0, 0);
          List_Move.Clear();
          List_Weapon.Clear();

          return this;
      }
  };



  public class VFXHelper_DrawRange
  {

    public int           DrawFlag         { get; private set; } = 0;
    public Int64         DrawEntityID     { get; private set; } = 0;
    public List<Int64>   VFXList          { get; private set; } = new List<Int64>();
    MoveRangeVisitor     MoveRangeVisitor { get; set; }         = new();

    public void DrawRange(int _draw_flag, Int64 _entityID)
    {
      // 이미 그려져있으면 처리하지 않는다.
      if (DrawFlag == _draw_flag && DrawEntityID == _entityID)
        return;

      Clear();

      // 그릴 범위가 없으면 처리하지 않는다.  
      if (_draw_flag == 0 || _entityID == 0)
        return;

      DrawFlag     = _draw_flag;
      DrawEntityID = _entityID;

      var terrain_map   = TerrainMapManager.Instance.TerrainMap;
      var entity_object = EntityManager.Instance.GetEntity(DrawEntityID);

      // 탐색.
      PathAlgorithm.FloodFill(
        MoveRangeVisitor.SetData(_draw_flag, terrain_map, entity_object));


      // 이동 범위 표시.
      foreach(var move in MoveRangeVisitor.List_Move)
      {
        var vfx_id = VFXManager.Instance.CreateVFXAsync(
            ObjectPool<VFXShape.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_EFFECT_BLUE)
            .SetPosition(move.CellToPosition())
        );

        VFXList.Add(vfx_id);
      }

      // 무기 범위 표시.
      foreach(var weapon in MoveRangeVisitor.List_Weapon)
      {
        // 이미 이동 범위에 포함되어 있으면 처리하지 않는다.
        if (MoveRangeVisitor.List_Move.Contains(weapon))
          continue;

        var vfx_id = VFXManager.Instance.CreateVFXAsync(
            ObjectPool<VFXShape.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_EFFECT_RED)
            .SetPosition(weapon.CellToPosition())
        );

        VFXList.Add(vfx_id);
      }     


    }

    public void Clear()
    {
      DrawFlag     = 0;
      DrawEntityID = 0;

      MoveRangeVisitor.Reset();
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