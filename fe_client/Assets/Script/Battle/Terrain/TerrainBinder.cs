using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class TerrainBinder : MonoBehaviour
    {
        [SerializeField]
        private Terrain          m_terrain           = null;


        [SerializeField]
        private FactionSerialize m_faction_serialize = null;

        [SerializeField]
        private Transform        m_root_fixed_objects = null;
    
    }
}


