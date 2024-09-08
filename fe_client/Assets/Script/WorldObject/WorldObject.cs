using System;
using UnityEngine;


public class WorldObject : MonoBehaviour
{
    Int64 m_id = 0;

    public Int64 ID => m_id;

    public void Initialize(Int64 _id)
    {
        m_id = _id;
    }

    
}