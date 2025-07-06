using System;
using Battle;
using UnityEngine;


public class WorldObject : MonoBehaviour
{
    
    public Int64 ID { get; private set; } = 0;

    private const float INTERPOLATION_INTERVAL = Constants.BATTLE_SYSTEM_UPDATE_INTERVAL;

    private Vector3 m_position          = Vector3.zero;
    private Vector3 m_position_prev     = Vector3.zero;
    private float   m_position_time     = 0f;


    // private Vector3 m_velocity              = Vector3.zero;
    // private float   m_position_updated_time = 0f;

    // private Vector3 m_position_visual       = Vector3.zero;
    // private Vector3 m_velocity_visual       = Vector3.zero;
    // private float   m_position_visual_time  = 0f;


    public bool Initialize(Entity _entity)
    {
        if (_entity == null)
            return false;

        ID = _entity.ID;

        
        SetPositionData(_entity.PathVehicle.Position, _entity.PathVehicle.PositionPrev, 0f);

        return true;
    }

    private void Update()
    {
        InterpolatePosition();
    }

    public void SetPositionData(Vector3 _position, Vector3 _position_prev, float _position_time)
    { 
        // 월드 오브젝트의 크기를 고려하여 위치 보정
        var half_size   = new Vector3(1f, 0f, 1f) * 0.5f;
        
        m_position      = _position + half_size;
        m_position_prev = _position_prev + half_size;
        m_position_time = _position_time;
    }

    private void InterpolatePosition()
    {
        var time_rate      = Mathf.Clamp01((Time.time - m_position_time) / INTERPOLATION_INTERVAL);
        transform.position = Vector3.Lerp(m_position_prev, m_position, time_rate);
    }    
}