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

        // PathNode에 셋팅된 위치로 이동을 합니다.
        var path_node         = _owner.PathNodeManager.Peek();        
        var target_position   = path_node.GetPosition();
        var target_rotation   = path_node.GetRotation();

        // 이동속도 & 회전을 셋팅합니다.
        var steering_velocity = (path_node.IsValidPosition()) ? target_position - m_position : Vector3.zero;
        var steering_torque   = (path_node.IsValidRotation()) ? target_rotation * Quaternion.Inverse(m_rotation) : Quaternion.identity;

        // 기존에 있던 속도 값은 제거합니다.
        steering_velocity -= m_velocity;
        steering_torque   *= Quaternion.Inverse(m_torque);

        return (steering_velocity, steering_torque);
    }
}
