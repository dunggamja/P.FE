using Battle;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TiledObject : MonoBehaviour
{
    [SerializeField]
    [Header("Entity ID, 17자리 이하까지만 사용해주세요.")]    
    private Int64 m_entity_id = 0;
}
