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

        public TerrainMap TerrainMap {get; private set; } = null;

        
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

        
            // 위치 갱신.
            TerrainMap.BlockManager.RefreshEntity(
                _event.EntityID, 
                _event.Cell_Prev.x, _event.Cell_Prev.y,
                _event.Cell_Cur.x, _event.Cell_Cur.y);                

            // ZOC 갱신.
            if (!_event.IgnorePrevCell)
                TerrainMap.ZOC.DecreaseZOC(_event.Faction, _event.Cell_Prev.x, _event.Cell_Prev.y);
                
            TerrainMap.ZOC.IncreaseZOC(_event.Faction, _event.Cell_Cur.x,  _event.Cell_Cur.y);
            
        }
    }
}