using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Unity.Profiling;
using UnityEngine;


public struct PathNode : IEquatable<PathNode>, IEqualityComparer<PathNode>
{
    float m_position_x; // ?œ„ì¹? X
    float m_position_z; // ?œ„ì¹? Z
    float m_rotation_y; // ë°©í–¥ Y

    public static PathNode Empty { get; } = new PathNode { m_position_x = -1f, m_position_z = -1f, m_rotation_y = -1f };

    public PathNode(float _position_x = -1f, float _position_z = -1f, float _rotation_y = -1f)
    {
        m_position_x = _position_x;
        m_position_z = _position_z;
        m_rotation_y = _rotation_y;
    }


    public bool       IsValidPosition() => 0f <= m_position_x && 0f <= m_position_z;
    public bool       IsValidRotation() => 0f <= m_rotation_y;

    public Vector3    GetPosition() => new Vector3(m_position_x, 0f, m_position_z);
    public Quaternion GetRotation() => Quaternion.Euler(0f, m_rotation_y * Mathf.Rad2Deg, 0f);

    public void SetPosition(float _x, float _z)
    {
        m_position_x = _x;
        m_position_z = _z;
    }

    public void SetPosition(Vector3 _position) => SetPosition(_position.x, _position.z);

    public void SetRotation(float _rotation_y)
    {
        m_rotation_y = _rotation_y;
    }

    public void SetRotation(Quaternion _rotation) => SetRotation(_rotation.eulerAngles.y * Mathf.Deg2Rad);

    public bool Equals(PathNode other)
    {
        return
            m_position_x == other.m_position_x &&
            m_position_z == other.m_position_z &&
            m_rotation_y == other.m_rotation_y;
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
            obj.m_rotation_y.GetHashCode();
    }



    // public static bool operator ==(PathNode x, PathNode y) =>  x.Equals(y);
    // public static bool operator !=(PathNode x, PathNode y) => !x.Equals(y);
}


public class PathNodeManager //: IPathNodeManager
{
    Queue<PathNode> m_list_path_node = new Queue<PathNode>();
    // PathNode        m_position_prev  = new PathNode();
    // PathNode        m_position_cur   = new PathNode();

    public TerrainMap TerrainMap => TerrainMapManager.Instance.TerrainMap;
    
    const float     m_arrive_radius  = 0.05f;
    // const float     m_arrive_angle   = Mathf.PI * (5f / 180f);

    public bool IsEmpty() => m_list_path_node.Count == 0;

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
        // Å¸°Ù À§Ä¡°¡ ÀÌ»ó.
        if (!_target.IsValidPosition())
            return true;


        var position_prev = _owner.PathVehicle.PositionPrev;
        var position      = _owner.PathVehicle.Position;

        
        var position_from_prev = _target.GetPosition() - position_prev;        
        var target_from_cur    = _target.GetPosition() - position;

        // µµÂø °Å¸® Ã¼Å©.
        if (target_from_cur.magnitude <= m_arrive_radius)
            return true;
        
        // À§Ä¡¸¦ Áö³ª°¬´ÂÁö Ã¼Å©.
        if (Vector3.Dot(position_from_prev, target_from_cur) < 0f)
            return true;
        
        return false;
    }

    // bool Check_Arrive_Rotation(PathNode _target, PathNode _prev, PathNode _current)
    // {
    //     return true;
    //     // È¸Àü Ã¼Å©ÇÒ °èÈ¹ ÀÏ´Ü ¾øÀ½.

    //     // // ?œ ?š¨?„± ì²´í¬
    //     // if (!_target.IsValidRotation())
    //     //     return true;

    //     // var rotation_target  = _target.GetRotation();
    //     // var rotation_current = _current.GetRotation();

    //     // // ?˜„?ž¬ ë°©í–¥ê³? ëª©í‘œ ë°©í–¥?˜ ì°¨ì´ë¥? ê³„ì‚°.
    //     // var rotation_diff  = rotation_target * Quaternion.Inverse(rotation_current);
    //     // var angle_diff     = Mathf.Acos(Quaternion.Dot(Quaternion.identity, rotation_diff));

    //     // // ë°©í–¥ ì°¨ì´ê°? ?—ˆ?š©ë²”ìœ„ ?‚´ ?ž„
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


    public bool CreatePath(Vector3 _from_position, Vector3 _dest_position, IPathOwner _path_owner)
    {
        if (TerrainMap == null)
            return false;

        ClearPath();
        
        var list_path_node = ListPool<PathNode>.Acquire();

        // °æ·Î Ã£±â.
        PathAlgorithm.PathFind(
            ref list_path_node, 
            TerrainMap, 
            _path_owner, 
            _from_position.PositionToCell(), 
            _dest_position.PositionToCell(),
            PathAlgorithm.PathFindOption.EMPTY
                .SetMoveRange(
                    true,
                    _path_owner.PathMoveRange, 
                    _path_owner.PathBasePosition)
            );        

        // º¹»ç.
        m_list_path_node.Clear();
        foreach (var node in list_path_node)
            m_list_path_node.Enqueue(node);

        ListPool<PathNode>.Return(list_path_node);

        
        return 0 < m_list_path_node.Count;
    }

    public void ClearPath()
    {
        m_list_path_node.Clear();
    }

}