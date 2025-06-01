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

        public TerrainMap TerrainMap {get; private set; } = null;

        
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
           

            // ��ġ&ZOC ����.
            if (_event.IsOccupy)
            {
                TerrainMap.BlockManager.AddEntity(_event.EntityID, _event.Cell.x,  _event.Cell.y);
                TerrainMap.ZOC.IncreaseZOC(_event.Faction, _event.Cell.x,  _event.Cell.y);
            }
            else
            {
                TerrainMap.BlockManager.RemoveEntity(_event.EntityID, _event.Cell.x, _event.Cell.y);
                TerrainMap.ZOC.DecreaseZOC(_event.Faction, _event.Cell.x, _event.Cell.y);                
            }
        }
    }
}