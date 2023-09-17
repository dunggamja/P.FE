using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PathNode : IEquatable<PathNode>, IEqualityComparer<PathNode>
{
    public float m_position_x;
    public float m_position_z;
    public float m_rotation_y;


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


public class PathNodeManager
{
    Queue<PathNode> m_list_path_node = new Queue<PathNode>();
    PathNode        m_position_prev  = new PathNode();
    PathNode        m_position_cur   = new PathNode();


    public void Initialize(Vector3 _position, float _rotation_y)
    {
        m_position_prev.m_position_x = m_position_cur.m_position_x = _position.x;
        m_position_prev.m_position_z = m_position_cur.m_position_z = _position.z;
        m_position_prev.m_rotation_y = m_position_cur.m_rotation_y = _rotation_y;
    }


    public void Update()
    {
        // 경로가 없음.
        if (m_list_path_node.Count == 0)
            return;

        // 위치가 변경된바가 없으면 
        if (m_position_prev.Equals(m_position_cur))
            return;


        var node   = m_list_path_node.Peek();
        
        // 이동 거리 = 현재 위치 - 이전 위치
        var moved  = new Vector3(m_position_cur.m_position_x - m_position_prev.m_position_x, 0f, m_position_cur.m_position_z - m_position_prev.m_position_z);

        // 타겟 거리 = 타겟 위치 - 이전 위치
        var target = new Vector3(node.m_position_x - m_position_prev.m_position_x, 0f, node.m_position_z - m_position_prev.m_position_z);



        

    }
}