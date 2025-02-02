using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathVehicle
{    
    protected Vector3    m_position_prev   = Vector3.zero;
    protected Vector3    m_position        = Vector3.zero;
    protected Vector3    m_velocity        = Vector3.zero;
    protected float      m_max_speed       = 0f;

    // protected Quaternion m_rotation_prev   = Quaternion.identity;
    // protected Quaternion m_rotation        = Quaternion.identity;
    // protected Quaternion m_torque          = Quaternion.identity;
    // protected float      m_max_angle_speed = 0f;


    public Vector3    Position     => m_position;
    // public Quaternion Rotation     => m_rotation;

    public Vector3    PositionPrev => m_position_prev;
    // public Quaternion RotationPrev => m_rotation_prev;


    protected abstract (Vector3 velocity, Quaternion torque) Steering(IPathOwner _owner);

    // TODO: velocity, max_speed 를 setup 할 일이 있을까....
    public void Setup(Vector3 _position, Vector3 _position_prev, Vector3 _velocity = default, float _max_speed = 5f)
    {
        m_position      = _position;
        m_position_prev = _position_prev;
        m_velocity      = _velocity;
        m_max_speed     = _max_speed;
    }


    // public void AddForce(Vector3 _force)
    // {
    //     m_velocity += _force;        
    // }

    // public void AddTorque(Quaternion _torque)
    // {
    //     m_torque *= _torque;        
    // }

    // public void SetMaxSpeed(float _max_speed) 
    // {
    //      m_max_speed = _max_speed;
    // }

    
    public void Update(IPathOwner _owner, float _delta_time)
    {

        (var steering_pos, var steering_rot) = Steering(_owner);
        m_velocity      = Truncate(steering_pos, m_max_speed);

        m_position_prev = m_position;
        m_position     += m_velocity * _delta_time;        

        // m_torque    *= steering_rot;
        // m_rotation  *= Quaternion.Euler(m_torque.eulerAngles * _delta_time);

        // Quaternion.Lerp(m_rotation, m_rotation * m_torque, m_max_angle_speed * _delta_time);
        // m_rotation = m_rotation * m_torque;        
    }


    Vector3 Truncate(Vector3 _vector, float _limit)
    {
        if (_limit < _vector.magnitude)
            return _vector.normalized * _limit;
        
        return _vector;
    }

}
