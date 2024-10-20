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

        // PathNode?�� ?��?��?�� ?��치로 ?��?��?�� ?��?��?��.
        var path_node         = _owner.PathNodeManager.Peek();        
        var target_position   = path_node.GetPosition();
        // var target_rotation   = path_node.GetRotation();


        var steering_velocity = Vector3.zero;

        if(path_node.IsValidPosition())        
        {
            steering_velocity = target_position - m_position;
            steering_velocity = steering_velocity.normalized * m_max_speed;
        }

        // var steering_torque   = (path_node.IsValidRotation()) ? target_rotation * Quaternion.Inverse(m_rotation) : Quaternion.identity;
        // steering_torque   *= Quaternion.Inverse(m_torque);

        // ȸ���� �ϴ� �Ű澲�� �ʴ´�.
        var steering_torque = Quaternion.identity;

        return (steering_velocity, steering_torque);
    }
}
