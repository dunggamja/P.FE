using System;
using UnityEngine;


public class WorldObject : MonoBehaviour
{
    
    public Int64 ID { get; private set; } = 0;


    private Vector3 m_position              = Vector3.zero;
    private Vector3 m_velocity              = Vector3.zero;
    private float   m_position_updated_time = 0f;

    private Vector3 m_position_visual       = Vector3.zero;
    private Vector3 m_velocity_visual       = Vector3.zero;
    private float   m_position_visual_time  = 0f;


    public void Initialize(Int64 _id)
    {
        ID = _id;
    }

    public void UpdatePosition(Vector3 _position, Vector3 _velocity, bool _sync_visual = false)
    {
        m_position              = _position;
        m_velocity              = _velocity;
        m_position_updated_time = Time.time;

        if (_sync_visual)
        {
            m_position_visual      = _position;
            m_velocity_visual      = _velocity;
            m_position_visual_time = Time.time;
        }
    }




    
}