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

  // 위치별로 체크하는 인터페이스, 1칸 이상 이동하는지 체크하고, 
  // 벽 넘어가지 않는지 체크하고,
  public interface IDrawRangeData
  {
    public int DrawFlag { get; }
    
    public HashSet<(int x, int y)> List_Move   { get; }
    public HashSet<(int x, int y)> List_Weapon { get; }
    
  }
  
  public class AttackRangeVisitor : 
      PathAlgorithm.IFloodFillVisitor, 
      IPoolObject, 
      IDrawRangeData
  {
    public int                DrawFlag     { get; set; } = 0;
    public TerrainMap         TerrainMap   { get; set; }
    public IPathOwner         Visitor      { get; set; }
    public (int x, int y)     Position     { get; set; }
    public int                MoveDistance { get; set; }
    public bool               VisitOnlyEmptyCell  => true;
    public bool               IsStop()            => false;

    public Int64              VisitorID    { get; set; } = 0;
    public (int min, int max) WeaponRange  { get; set; } = (0, 0);

    public HashSet<(int x, int y)> List_Move   { get; set; } = new();
    public HashSet<(int x, int y)> List_Weapon { get; set; } = new();



    public void Visit(int _visit_x, int _visit_y)
    {
        // bool result = false;

        List_Move.Add((_visit_x, _visit_y));
        
        var weapon_range_min = WeaponRange.min;
        var weapon_range_max = WeaponRange.max;

        

        // 범위 체크.
        for(int x = -weapon_range_max; x <= weapon_range_max; ++x)
        {
            for(int y = -weapon_range_max; y <= weapon_range_max; ++y)
            {
                var weapon_x = _visit_x + x;
                var weapon_y = _visit_y + y;

                // 범위 체크.
                var distance = PathAlgorithm.Distance(_visit_x, _visit_y, weapon_x, weapon_y);
                if (distance < weapon_range_min || weapon_range_max < distance)
                {
                    continue;
                }

                // 범위 체크.
                if (TerrainMap != null && TerrainMap.IsInBound(weapon_x, weapon_y) == false)
                {
                  continue;
                }

                List_Weapon.Add((weapon_x, weapon_y));

                // result = true;
            }
        }

        // return result;
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
        List_Move.Clear();
        List_Weapon.Clear();
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


      // 이동 범위 체크.
      foreach(var move in AttackRangeVisitor.List_Move)
      {
        var vfx_id = VFXManager.Instance.CreateVFXAsync(
            ObjectPool<VFXShape.Param>.Acquire()
            .SetVFXRoot_Default()
            .SetVFXName(AssetName.TILE_EFFECT_BLUE)
            .SetPosition(move.CellToPosition())
        );

        VFXList.Add(vfx_id);
      }

      // 공격 범위 체크.
      foreach(var weapon in AttackRangeVisitor.List_Weapon)
      {
        // 이동 범위 체크.
        if (AttackRangeVisitor.List_Move.Contains(weapon))
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