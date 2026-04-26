using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Unity.Profiling;
using UnityEngine;


public struct PathNode : IEquatable<PathNode>, IEqualityComparer<PathNode>
{
    float m_position_x; // 위치 X
    float m_position_z; // 위치 Z
    // float m_rotation_y; // 방향 Y

    int   m_move_cost;


    public int MoveCost => m_move_cost;

    public static PathNode Empty { get; } = new PathNode
     { 
        m_position_x = -1f, 
        m_position_z = -1f, 
        // m_rotation_y = -1f, 
        m_move_cost = 0 
    };

    public PathNode(
      float _position_x = -1f
    , float _position_z = -1f
    // , float _rotation_y = -1f
    , int _move_cost = 0)
    {
        m_position_x = _position_x;
        m_position_z = _position_z;
        // m_rotation_y = _rotation_y;
        m_move_cost  = _move_cost;
    }


    public bool       IsValidPosition() => 0f <= m_position_x && 0f <= m_position_z;
    // public bool       IsValidRotation() => 0f <= m_rotation_y;

    public Vector3    GetPosition() => new Vector3(m_position_x, 0f, m_position_z);
    // public Quaternion GetRotation() => Quaternion.Euler(0f, m_rotation_y * Mathf.Rad2Deg, 0f);

    public void SetPosition(float _x, float _z)
    {
        m_position_x = _x;
        m_position_z = _z;
    }

    public void SetPosition(Vector3 _position) => SetPosition(_position.x, _position.z);

    // public void SetRotation(float _rotation_y)
    // {
    //     m_rotation_y = _rotation_y;
    // }

    // public void SetRotation(Quaternion _rotation) => SetRotation(_rotation.eulerAngles.y * Mathf.Deg2Rad);

    public bool Equals(PathNode other)
    {
        return
            m_position_x == other.m_position_x &&
            m_position_z == other.m_position_z &&
            // m_rotation_y == other.m_rotation_y &&
            m_move_cost  == other.m_move_cost;
    }

    public bool Equals(PathNode x, PathNode y)
    {
        return x.Equals(y);
    }

    public int GetHashCode(PathNode obj)
    {
        return 
            obj.m_position_x.GetHashCode() ^ 
            obj.m_position_z.GetHashCode() ^ 
            // obj.m_rotation_y.GetHashCode() ^
            obj.m_move_cost.GetHashCode();
    }

    // public static bool operator ==(PathNode x, PathNode y) =>  x.Equals(y);
    // public static bool operator !=(PathNode x, PathNode y) => !x.Equals(y);
}




public class PathNodeManager //: IPathNodeManager
{
    Queue<PathNode> m_list_path_node    = new Queue<PathNode>();
    List<PathNode>  m_list_arrived_node = new List<PathNode>();
    

    public TerrainMap TerrainMap => TerrainMapManager.Instance.TerrainMap;
    
    const float     m_arrive_radius  = 0.05f;
    // const float     m_arrive_angle   = Mathf.PI * (5f / 180f);

    public bool IsEmpty() => m_list_path_node.Count == 0;


    // 도착한 경로 노드 목록을 반환합니다.
    public List<PathNode> GetArrivedPathNodes()
    {
        return m_list_arrived_node;
    }

    // 도착한 경로까지의 거리를 반환합니다.
    public int GetArrivedPathDistance() 
    {
        if (m_list_arrived_node.Count == 0)
            return 0;


        int count     = 0;
        var prev_cell = m_list_arrived_node[0].GetPosition().PositionToCell();

        foreach (var node in m_list_arrived_node)
        {
            var cur_cell = node.GetPosition().PositionToCell();
            if (cur_cell != prev_cell)
            {
                count    += PathAlgorithm.Distance(prev_cell, cur_cell);
                prev_cell = cur_cell;
            }
        }

        return count;
    }

    /// <summary>대기 중인 경로 노드를 순서대로 복사합니다(큐는 변경하지 않음).</summary>
    public void CopyRemainingPathNodes(List<PathNode> _out)
    {
        if (_out == null)
            return;

        _out.Clear();
        foreach (var node in m_list_path_node)
            _out.Add(node);
    }

    /// <summary>대기 경로의 마지막 칸(도착 예정 셀)을 반환합니다.</summary>
    public bool TryGetQueuedPathDestinationCell(out (int x, int y) _cell)
    {
        _cell = default;
        if (m_list_path_node.Count == 0)
            return false;

        PathNode last = PathNode.Empty;
        foreach (var node in m_list_path_node)
            last = node;

        _cell = last.GetPosition().PositionToCell();
        return true;
    }

    public void Update(IPathOwner _owner)
    {
        if (_owner == null)
            return;

        // 
        if (m_list_path_node.Count == 0)
            return;

        while(m_list_path_node.Count > 0)
        {
            var node   = m_list_path_node.Peek();               

            // && Check_Arrive_Rotation(node, m_position_prev, m_position_cur)

            if (Check_Arrive_Position(node, _owner))             
            {
                m_list_arrived_node.Add(node);
                // 
                m_list_path_node.Dequeue();
            }
            else
            {
                // 
                break;
            }
        }


    }

    bool Check_Arrive_Position(PathNode _target, IPathOwner _owner)
    {
        // 위치 체크.
        if (!_target.IsValidPosition())
            return true;


        var position_prev = _owner.PathVehicle.PositionPrev;
        var position      = _owner.PathVehicle.Position;

        
        var position_from_prev = _target.GetPosition() - position_prev;        
        var target_from_cur    = _target.GetPosition() - position;

        // 위치 차이 체크.
        if (target_from_cur.magnitude <= m_arrive_radius)
            return true;
        
        // 위치 차이 체크.
        if (Vector3.Dot(position_from_prev, target_from_cur) < 0f)
            return true;
        
        return false;
    }

    // bool Check_Arrive_Rotation(PathNode _target, PathNode _prev, PathNode _current)
    // {
    //     return true;
    //     // 방향 체크 여부.

    //     // // 방향 체크
    //     // if (!_target.IsValidRotation())
    //     //     return true;

    //     // var rotation_target  = _target.GetRotation();
    //     // var rotation_current = _current.GetRotation();

    //     // // 방향 차이 계산.
    //     // var rotation_diff  = rotation_target * Quaternion.Inverse(rotation_current);
    //     // var angle_diff     = Mathf.Acos(Quaternion.Dot(Quaternion.identity, rotation_diff));

    //     // // 방향 차이 범위 체크.
    //     // if (angle_diff <= m_arrive_angle)
    //     //     return true;

    //     // return false;
    // }

    public PathNode Peek()
    {
        if (0 < m_list_path_node.Count)
            return m_list_path_node.Peek();

        return PathNode.Empty;
    }


    public bool CreatePath(
        Vector3    _from_position, 
        Vector3    _dest_position, 
        IPathOwner _path_owner,
        bool       _is_force_move = false)
    {
        if (TerrainMap == null)
            return false;

        ClearPath();


        var option = PathAlgorithm.PathFindOption.Create();

        if (_is_force_move)
        { 
            // 이동 비용/ ZOC를 무시하고 길찾기를 진행합니다.
            option.SetMoveForced();            
        }
        else
        { 
            // 이동 가능한 범위를 벗어나지 않기위해 (범위,위치) 셋팅
            option.SetMoveLimitRange(_path_owner.PathMoveRange, _path_owner.PathBasePosition);
        }

        
        var list_path_node = ListPool<PathNode>.Acquire();

        // 경로 찾기.
        var path_find = PathAlgorithm.PathFind(
            
            TerrainMap, 
            _path_owner, 

            // 출발지/도착지.
            _from_position.PositionToCell(), 
            _dest_position.PositionToCell(),

            // 길찾기 경로 옵션값.
            option,

            // 길찾기 경로.
            list_path_node
            ).result;        

        // 경로 저장.
        if (path_find)
        {
            ClearPath();
            
            foreach (var node in list_path_node)
                m_list_path_node.Enqueue(node);
        }

        ListPool<PathNode>.Return( list_path_node);

        
        return path_find;
    }

    public void ClearPath()
    {
        m_list_path_node.Clear();
        m_list_arrived_node.Clear();
    }

}