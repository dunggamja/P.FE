using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace Battle
{
    public class BattleTerrainManager : Singleton<BattleTerrainManager>
    {
        public BattleTerrain BattleTerrain {get; private set; } = null;

        public void Initialize()
        {
            // 일단 하드코딩.  
            BattleTerrain = new BattleTerrain();
            BattleTerrain.Initialize(100, 100);
        }


        public void Reset()
        {
            BattleTerrain = null;
        }
    }
}