using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


namespace Battle
{
    [EventReceiver(typeof(Battle_Cell_OccupyEvent))]
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
                case Battle_Cell_OccupyEvent cell_event:
                    OnReceiveEvent_CellPositionEvent(cell_event);
                    break;
            }
        }

        void OnReceiveEvent_CellPositionEvent(Battle_Cell_OccupyEvent _event)
        {
             if (TerrainMap == null)
                return;

            if (_event == null)
                return;
           

            // 점유 여부는 SET 데이터이므로 준대로 갱신처리.
            if (_event.IsOccupy.cur)
            {
                TerrainMap.EntityManager.SetCellData(_event.Cell.x,  _event.Cell.y, _event.EntityID);
            }
            else
            {
                TerrainMap.EntityManager.RemoveCellData(_event.Cell.x, _event.Cell.y);
            }

            // 유닛의 ZOC 상태 변경 또는 점유상태 변경 체크.
            if (_event.IsIncreaseZOC)
            {
                TerrainMap.ZOC.IncreaseZOC(_event.Faction, _event.Cell.x, _event.Cell.y);
            }
            else if (_event.IsDecreaseZOC)
            {
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