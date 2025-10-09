using UnityEngine;
using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class FixedObjects : MonoBehaviour
{
    // 
    [SerializeField]
    [Header("Entity ID, 17자리 이하까지만 사용해주세요.")]    
    private Int64 m_entity_id = 0;


}


