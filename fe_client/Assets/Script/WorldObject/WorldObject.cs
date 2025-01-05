using System;
using Battle;
using UnityEngine;


public class WorldObject : MonoBehaviour
{
    
    public Int64 ID { get; private set; } = 0;

    private const float INTERPOLATION_INTERVAL = 0.15f;

    private Vector3 m_position          = Vector3.zero;
    private Vector3 m_position_prev     = Vector3.zero;
    private float   m_position_time     = 0f;


    // private Vector3 m_velocity              = Vector3.zero;
    // private float   m_position_updated_time = 0f;

    // private Vector3 m_position_visual       = Vector3.zero;
    // private Vector3 m_velocity_visual       = Vector3.zero;
    // private float   m_position_visual_time  = 0f;


    public void Initialize(Int64 _id)
    {
        ID = _id;
    }

    private void Update()
    {
        InterpolatePosition();
    }

    public void SetPositionData(Vector3 _position, Vector3 _position_prev, float _position_time)
    {
        m_position      = _position;
        m_position_prev = _position_prev;
        m_position_time = _position_time;
    }

    private void InterpolatePosition()
    {
        var time_rate      = Mathf.Clamp01((Time.time - m_position_time) / INTERPOLATION_INTERVAL);
        transform.position = Vector3.Lerp(m_position_prev, m_position, time_rate);
    }    
}