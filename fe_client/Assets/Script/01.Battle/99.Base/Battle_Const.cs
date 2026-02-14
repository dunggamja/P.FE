using System;


namespace Battle
{
    public static class Constants
    {
        public const int   MAX_MAP_SIZE = 1000;
        public const float BATTLE_SYSTEM_UPDATE_INTERVAL = 1f/30f;
        public const float BATTLE_MOVE_SPEED_MAX         = 10f;

        public const float BATTLE_VFX_SNAP_OFFSET_TILE   = 0.1f;

        // public const string TAG_ROOT = "TAG_ROOT";

        public const string TAG_BATTLE_TERRAIN = "Battle_Terrain";


        public const string LAYER_TERRAIN      = "Terrain";
        public const string LAYER_FIXED_OBJECT = "FixedObject";


        // 갈수없는 지형인데 강제로 이동시켜야 할경우 사용하는 값(연출등)
        public const int    PENALTY_TERRAIN_COST = 30;








    }

}