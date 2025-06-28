using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

namespace Battle.MoveRange
{
  public enum EnumDrawFlag
  {    
    MoveRange   = 1 << 0,    // �̵� �Ÿ�
    AttackRange = 1 << 1,  // ���� ����
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

          

          // ���� ��Ÿ� ����.
          for(int x = -weapon_range_max; x <= weapon_range_max; ++x)
          {
              for(int y = -weapon_range_max; y <= weapon_range_max; ++y)
              {
                  var weapon_x = _visit_x + x;
                  var weapon_y = _visit_y + y;

                  // ���� ��Ÿ� üũ
                  var distance = PathAlgorithm.Distance(_visit_x, _visit_y, weapon_x, weapon_y);
                  if (distance < weapon_range_min || weapon_range_max < distance)
                  {
                      continue;
                  }

                  // �� ���� üũ.
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
      // �̹� �׷��������� ó������ �ʴ´�.
      if (DrawFlag == _draw_flag && DrawEntityID == _entityID)
        return;

      Clear();

      // �׸� ������ ������ ó������ �ʴ´�.  
      if (_draw_flag == 0 || _entityID == 0)
        return;

      DrawFlag     = _draw_flag;
      DrawEntityID = _entityID;

      var terrain_map   = TerrainMapManager.Instance.TerrainMap;
      var entity_object = EntityManager.Instance.GetEntity(DrawEntityID);

      // Ž��.
      PathAlgorithm.FloodFill(
        MoveRangeVisitor.SetData(_draw_flag, terrain_map, entity_object));


      // �̵� ���� ǥ��.
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

      // ���� ���� ǥ��.
      foreach(var weapon in MoveRangeVisitor.List_Weapon)
      {
        // �̹� �̵� ������ ���ԵǾ� ������ ó������ �ʴ´�.
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