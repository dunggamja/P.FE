using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace Battle
{
    public class TerrainMapManager : Singleton<TerrainMapManager>
    {
        public TerrainMap TerrainMap {get; private set; } = null;

        public void Initialize()
        {
            // 
            TerrainMap = new TerrainMap();
            TerrainMap.Initialize(100, 100);
        }


        public void Reset()
        {
            TerrainMap = null;
        }
    }
}