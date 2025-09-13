using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  
  public class BlockManager
  {
    public struct Node //: IPoolObject
    {
      int            m_block_x    ;//    = 0;
      int            m_block_y    ;//    = 0;
      int            m_block_size ;//    = 0;
      HashSet<Int64> m_list_id    ;//   = new();    

      public (int x, int y) BlockPosition => (m_block_x, m_block_y);

      public HashSet<Int64> ListId => m_list_id;

      public void AddId(Int64 _id)
      {
        if (m_list_id == null)
            m_list_id = new();

        m_list_id.Add(_id);
      }

      public void RemoveId(Int64 _id)
      {
        if (m_list_id == null)
            return;

        m_list_id.Remove(_id);
      }


    }


    private int                               m_world_width  = 0;
    private int                               m_world_height = 0;
    private int                               m_block_size   = 0;
    private Node[,]                           m_nodes        = null;
    private Dictionary<Int64, (int x, int y)> m_id_position  = new();


    public void Initialize(int _world_width, int _world_height, int _block_size)
    {
      m_world_width   = _world_width;
      m_world_height  = _world_height;
      m_block_size    = _block_size;

      var world_size  = Math.Max(m_world_width, m_world_height);
      var block_count = (world_size / m_block_size);      
      if ((world_size % m_block_size) > 0)
        block_count++;

      m_nodes         = new Node[block_count, block_count];
      m_id_position   = new();
    }


    (int x, int y) PositionToBlockIndex(int _pos_x, int _pos_y)
    {
      if (m_block_size <= 0)
        return (-1, -1);

      var block_x = _pos_x / m_block_size;
      var block_y = _pos_y / m_block_size;

      return (block_x, block_y);
    }

    public void SetID(int _pos_x, int _pos_y, Int64 _id)
    {
      var (block_x, block_y) = PositionToBlockIndex(_pos_x, _pos_y);
      if (block_x < 0 || block_y < 0)
        return;


      // 이미 있는 항목 제거.
      if (HasID(_id))
          RemoveID(_id);

      
      // 
      m_nodes[block_x, block_y].AddId(_id);
      m_id_position[_id] = (_pos_x, _pos_y);
    }

    public void RemoveID(Int64 _id)
    {
      if (!m_id_position.TryGetValue(_id, out var position))
        return;

      var (block_x, block_y) = PositionToBlockIndex(position.x, position.y);
      if  (block_x >= 0 && block_y >= 0)
        m_nodes[block_x, block_y].RemoveId(_id);

      m_id_position.Remove(_id);
    }

    public bool HasID(Int64 _id)
    {
      return m_id_position.ContainsKey(_id);
    }

    (int x, int y) GetIDPosition(Int64 _id)
    {
      if (!m_id_position.TryGetValue(_id, out var position))
        return (-1, -1);

      return position;
    }

    public void Query_AABB(
        List<Int64>                _results,
        AABB                       _box, 
        SpacePartition.QueryFilter _query_filter = null)
    {
      var block_index_min = PositionToBlockIndex((int)_box.min.x, (int)_box.min.y);
      var block_index_max = PositionToBlockIndex((int)_box.max.x, (int)_box.max.y);

      for (var x = block_index_min.x; x <= block_index_max.x; x++)
      {
        for (var y = block_index_min.y; y <= block_index_max.y; y++)
        {
          foreach (var id in m_nodes[x, y].ListId)
          {
            var position = GetIDPosition(id);
            if (_query_filter == null || _query_filter(position, id))
                _results.Add(id);            
          }
        }
      }
    }

    public void Query_Center(
      List<Int64>                _results, 
      (int x, int y)             _center, 
      int                        _range,
      SpacePartition.QueryFilter _query_filter = null)
    {
      var min = new Vector2(_center.x - _range, _center.y - _range);
      var max = new Vector2(_center.x + _range, _center.y + _range);  

      var block_index_min = PositionToBlockIndex((int)min.x, (int)min.y);
      var block_index_max = PositionToBlockIndex((int)max.x, (int)max.y);

      for (var x = block_index_min.x; x <= block_index_max.x; x++)
      {
        for (var y = block_index_min.y; y <= block_index_max.y; y++)
        {
          foreach (var id in m_nodes[x, y].ListId)
          {
            // 거리 체크.
            var position = GetIDPosition(id);
            var distance = PathAlgorithm.Distance(_center.x, _center.y, position.x, position.y);
            if (distance > _range)
                continue;

            if (_query_filter == null || _query_filter(position, id))
                _results.Add(id);            
          }
        }
      }
    }

  }
}