using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace Battle
{
    [EventReceiver(typeof(Battle_Cell_PositionEvent))]
    public class TerrainMapManager : Singleton<TerrainMapManager>, IEventReceiver
    {

        protected override void Init()
        {
            base.Init();

            // event receiver 
            EventDispatchManager.Instance.AttachReceiver(this);
        }

        private TerrainBinder m_terrain_binder = null;

        public  TerrainMap    TerrainMap    { get; private set; } = null;
        public  TerrainBinder TerrainBinder 
        { 
            get
            {
                if (m_terrain_binder == null)
                {
                    m_terrain_binder = GameObject.FindGameObjectWithTag(Battle.Constants.TAG_BATTLE_TERRAIN).GetComponent<TerrainBinder>();
                }

                return m_terrain_binder;
            }
        }

        // TODO: 현재로서는 임시방편용 코드에 가까움. 맵에 높이값도 저장하도록 바꿀거임.

        // private Terrain    WorldTerrain
        // {
        //     get
        //     {
        //         if (m_world_terrain == null)
        //         {
        //             var world_terrain_object = GameObject.FindGameObjectWithTag(Battle.Constants.TAG_BATTLE_TERRAIN);
        //             if (world_terrain_object != null)
        //             {
        //                 m_world_terrain = world_terrain_object.GetComponent<Terrain>();
        //             }
        //             else
        //             {
        //                 Debug.LogError($"WorldTerrain not found");
        //             }
        //         }

        //         return m_world_terrain;
        //     }
        // }

        public float GetWorldHeight((int _x, int _y) _cell, bool _exclude_fixedobjects = false)
        {
            var world_position  = _cell.CellToPosition();
            world_position.x   += 0.5f;
            world_position.z   += 0.5f;

            return GetWorldHeight(world_position, _exclude_fixedobjects);
        }

        public float GetWorldHeight(Vector3 _world_position, bool _exclude_fixedobjects = false)
        {
            if (TerrainBinder != null)
                return TerrainBinder.GetHeight(_world_position, _exclude_fixedobjects);

            return 0f;
        }
        
        public void SetTerrainMap(TerrainMap _terrain_map)
        {
            TerrainMap = _terrain_map;
        }

        public void OnReceiveEvent(IEventParam _event)
        {
            switch (_event)
            {
                case Battle_Cell_PositionEvent cell_event:
                    OnReceiveEvent_CellPositionEvent(cell_event);
                    break;
            }
        }

        void OnReceiveEvent_CellPositionEvent(Battle_Cell_PositionEvent _event)
        {
             if (TerrainMap == null)
                return;

            if (_event == null)
                return;
           

            // 위치 & ZOC 갱신.
            if (_event.IsOccupy)
            {
                TerrainMap.EntityManager.SetCellData(_event.Cell.x,  _event.Cell.y, _event.EntityID);
                TerrainMap.ZOC.IncreaseZOC(_event.Faction, _event.Cell.x,  _event.Cell.y);
            }
            else
            {
                TerrainMap.EntityManager.RemoveCellData(_event.Cell.x, _event.Cell.y);
                TerrainMap.ZOC.DecreaseZOC(_event.Faction, _event.Cell.x, _event.Cell.y);                
            }
        }

        public TerrainMapManager_IO Save()
        {
            return new TerrainMapManager_IO 
            { 
                TerrainMap = TerrainMap.Save() 
            };
        }

        public void Load(TerrainMapManager_IO _io, bool _is_plan)
        {
            TerrainMap.Load(_io.TerrainMap);
        }
    }


    public class TerrainMapManager_IO
    {
        public TerrainMap_IO TerrainMap { get; set; }
    }
}