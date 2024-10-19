using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;


public struct PathNode : IEquatable<PathNode>, IEqualityComparer<PathNode>
{
    float m_position_x; // ?���? X
    float m_position_z; // ?���? Z
    float m_rotation_y; // 방향 Y

    public static PathNode Empty { get; } = new PathNode { m_position_x = 0, m_position_z = 0, m_rotation_y = 0 };

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
    PathNode        m_position_prev  = new PathNode();
    PathNode        m_position_cur   = new PathNode();

    public TerrainMap TerrainMap => TerrainMapManager.Instance.TerrainMap;
    
    const float     m_arrive_radius  = 0.05f;
    // const float     m_arrive_angle   = Mathf.PI * (5f / 180f);



    public void Setup(Vector3 _position, float _rotation_y)
    {
        m_position_prev.SetPosition(_position);
        m_position_cur.SetPosition(_position); 
        
        m_position_prev.SetRotation(_rotation_y);
        m_position_cur.SetRotation(_rotation_y);     
    }


    public void Update()
    {
        // 
        if (m_list_path_node.Count == 0)
            return;

        // 
        if (m_position_prev.Equals(m_position_cur))
            return;

        while(m_list_path_node.Count > 0)
        {
            var node   = m_list_path_node.Peek();               

            // && Check_Arrive_Rotation(node, m_position_prev, m_position_cur)

            if (Check_Arrive_Position(node, m_position_prev, m_position_cur))             
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

    bool Check_Arrive_Position(PathNode _target, PathNode _prev, PathNode _current)
    {
        // Ÿ�� ��ġ�� �̻�.
        if (!_target.IsValidPosition())
            return true;

        
        var position_from_prev = _target.GetPosition() - _prev.GetPosition();        
        var target_from_cur    = _target.GetPosition() - _current.GetPosition();

        // ���� �Ÿ� üũ.
        if (target_from_cur.magnitude <= m_arrive_radius)
            return true;
        
        // ��ġ�� ���������� üũ.
        if (Vector3.Dot(position_from_prev, target_from_cur) < 0f)
            return true;
        
        return false;
    }

    // bool Check_Arrive_Rotation(PathNode _target, PathNode _prev, PathNode _current)
    // {
    //     return true;
    //     // ȸ�� üũ�� ��ȹ �ϴ� ����.

    //     // // ?��?��?�� 체크
    //     // if (!_target.IsValidRotation())
    //     //     return true;

    //     // var rotation_target  = _target.GetRotation();
    //     // var rotation_current = _current.GetRotation();

    //     // // ?��?�� 방향�? 목표 방향?�� 차이�? 계산.
    //     // var rotation_diff  = rotation_target * Quaternion.Inverse(rotation_current);
    //     // var angle_diff     = Mathf.Acos(Quaternion.Dot(Quaternion.identity, rotation_diff));

    //     // // 방향 차이�? ?��?��범위 ?�� ?��
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


    public bool CreatePath(Vector3 _dest_position, float _dest_angle_degree, IPathOwner _path_owner)
    {
        if (TerrainMap == null)
            return false;


        var from_position  = m_position_cur.GetPosition();
        var list_path_node = PathAlgorithm.PathFind(TerrainMap, _path_owner, (int)from_position.x, (int)from_position.z, (int)_dest_position.x, (int)_dest_position.z);
        
        m_list_path_node.Clear();
        m_list_path_node = new Queue<PathNode>(list_path_node);

        
        return 0 < m_list_path_node.Count;
    }

}