using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Battle
{
    public class TerrainBaker : MonoBehaviour
    {
       #if UNITY_EDITOR
        [SerializeField]
        private TerrainBinder m_terrain_binder = null;
            
        [SerializeField]      
        private Tilemap       m_tilemap        = null;
       #endif
    }
}