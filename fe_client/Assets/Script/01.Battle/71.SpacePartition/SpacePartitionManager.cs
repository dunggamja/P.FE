using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace Battle
{
  // public enum EnumSpacePartitionType
  // {
  //   None,
  //   Threat,   // 이동거리 + 공격범위.    
  //   Position, // 위치.
  // }



   [EventReceiver(typeof(Battle_Cell_OccupyEvent))]
  public class SpacePartitionManager : Singleton<SpacePartitionManager>, IEventReceiver
  {
     
    
     public Dictionary<int, DynamicAABBTree>  FactionThreaten { get; private set; } = new();
     public BlockManager    EntityPosition { get; private set; } = new();

     const int BLOCK_SIZE = 8;

    protected override void Init()
    {
        base.Init();
        EventDispatchManager.Instance.AttachReceiver(this);
    }


    public void Initialize(int _world_width, int _world_height)
     {
        FactionThreaten.Clear();
        EntityPosition.Initialize(_world_width, _world_height, BLOCK_SIZE);
     }

     public void OnReceiveEvent(IEventParam _event)
     {
        switch (_event)
        {
            case Battle_Cell_OccupyEvent cell_position_event:
                OnReceiveEvent_CellPositionEvent(cell_position_event);
                break;
        }
     }

     void OnReceiveEvent_CellPositionEvent(Battle_Cell_OccupyEvent _event)
     {
        if (_event == null)
          return;

        // 점유 여부는 SET 데이터이므로 준대로 갱신처리.
        if (_event.IsOccupy.cur)
        {
          UpdateEntity(_event.EntityID);
        }
        else
        {
          RemoveEntity(_event.EntityID);
        }
     }



     public void UpdateEntity(Int64 _id)
     {
        var entity = EntityManager.Instance.GetEntity(_id);
        if (entity == null)
          return;


        // 위협 범위 갱신.
        UpdateThreatenRange(entity);

        // 위치 갱신.
        EntityPosition.SetID(entity.Cell.x, entity.Cell.y, entity.ID);
     }

     public void RemoveEntity(Int64 _id)
     {
        foreach(var e in FactionThreaten.Values)
          e.Remove(_id);

        EntityPosition.RemoveID(_id);
     }

     void UpdateThreatenRange(Entity _entity)
     {
        if (_entity == null)
          return;

        // 기존 데이터 제거.
        foreach(var e in FactionThreaten.Values)
          e.Remove(_entity.ID);

        // 진영에 맞는 AABB트리 가져오기.
        var faction = _entity.GetFaction();
        if (FactionThreaten.TryGetValue(faction, out var tree) == false)
        {
          tree = new DynamicAABBTree();
          FactionThreaten.Add(faction, tree);
        }

        // 센터 +- (이동거리 + 공격범위) = AABB 크기.
        var center = _entity.Cell;
        var range  = _entity.PathMoveRange + _entity.GetWeaponRange().max;

        var box    = new AABB {
          min = new Vector2(center.x - range, center.y - range),
          max = new Vector2(center.x + range, center.y + range)
        };
        
        // AABB트리에 삽입.
        tree.Insert(_entity.ID, box);
     }


     public void Query_Position_AABB(
        List<Int64>                _results,
        AABB                       _box,
        SpacePartition.QueryFilter _filter = null)
     {
        EntityPosition.Query_AABB(_results, _box, _filter);
     }

     public void Query_Position_Range(
        List<Int64>                _results,
        (int x, int y)             _center,
        int                        _range,
        SpacePartition.QueryFilter _filter = null)
     {
        EntityPosition.Query_Center(_results, _center, _range, _filter);
     }

    //  public void Query_Threaten(
    //     List<Int64>                _results,
    //     AABB                       _box,
    //     SpacePartition.QueryFilter _filter = null)
    //  {
    //     EntityPosition.RangeAABB(_results, _box, _filter);
    //  }
  }
}