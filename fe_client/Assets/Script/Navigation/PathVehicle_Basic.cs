using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVehicle_Basic : PathVehicle
{
    protected override (Vector3 velocity, Quaternion torque) Steering(IPathOwner _owner)
    {
        if (_owner == null || _owner.PathNodeManager == null)
            return (Vector3.zero, Quaternion.identity);

        // PathNode? ??? ?μΉλ‘ ?΄?? ?©??€.
        var path_node         = _owner.PathNodeManager.Peek();        
        var target_position   = path_node.GetPosition();
        // var target_rotation   = path_node.GetRotation();

        // ?΄??? & ?? ? ???©??€.
        var steering_velocity = (path_node.IsValidPosition()) ? target_position - m_position : Vector3.zero;
        // var steering_torque   = (path_node.IsValidRotation()) ? target_rotation * Quaternion.Inverse(m_rotation) : Quaternion.identity;

        // κΈ°μ‘΄? ?? ?? κ°μ?? ? κ±°ν©??€.
        steering_velocity -= m_velocity;
        // steering_torque   *= Quaternion.Inverse(m_torque);

        // ΘΈΐόΐΊ ΐΟ΄ά ½Ε°ζΎ²Αφ ΎΚ΄Β΄Ω.
        var steering_torque = Quaternion.identity;

        return (steering_velocity, steering_torque);
    }
}
