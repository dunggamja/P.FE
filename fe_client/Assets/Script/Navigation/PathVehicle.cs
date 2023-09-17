using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVehicle
{
    public delegate Vector3 SteeringDelegate();

    Vector3          m_position          = Vector3.zero;
    Vector3          m_velocity          = Vector3.zero;
    float            m_max_speed         = 0f;
    SteeringDelegate m_steering_function = null;



    public Vector3 Position => m_position;

    public void AddForce(Vector3 _force)
    {
        m_velocity += _force;        
    }

    public void SetMaxSpeed(float _max_speed) 
    {
         m_max_speed = _max_speed;
    }

    public void SetSteeringFunction(SteeringDelegate _steering_function)
    {
        m_steering_function = _steering_function;
    }


    public void Update(float _delta_time)
    {
        var steering = m_steering_function?.Invoke() ?? Vector3.zero;

        m_velocity   = Truncate(m_velocity + steering, m_max_speed);
        m_position  += m_velocity * _delta_time;        
    }


    Vector3 Truncate(Vector3 _vector, float _limit)
    {
        if (_limit < _vector.magnitude)
            return _vector.normalized * _limit;
        
        return _vector;
    }

}
