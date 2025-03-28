using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SituationUpdatedEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;
        public EnumSituationType      Situation   { get; private set; }  = EnumSituationType.None;
        // public ISystemParam       SystemParam { get; private set; }   

        public SituationUpdatedEvent Set(EnumSituationType _situation_type)
        {
            Situation   = _situation_type;
            // SystemParam = _system_param;
            return this;
        }

        public void Reset()
        {
            Situation = EnumSituationType.None;
        }

        public void Release()
        {
            ObjectPool<SituationUpdatedEvent>.Return(this);
        }
    }

    public class AIUpdateEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;

        public float TopScore          { get; private set; } = 0;
        public Int64 TopScore_EntityID { get; private set; } = 0;

        public void TryTopScore(Int64 _entity_id, float _score)
        {
            if (TopScore < _score)
            {
                TopScore          = _score;
                TopScore_EntityID = _entity_id;
            }
        }

        public void Reset()
        {
            TopScore          = 0;
            TopScore_EntityID = 0;
        }

        public void Release()
        {
            ObjectPool<AIUpdateEvent>.Return(this);
        }
    }

    public class CellPositionEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;

        public Int64          EntityID       { get; private set; } = 0;
        public int            Faction        { get; private set; } = 0;
        public (int x, int y) Cell_Cur       { get; private set; } = (0, 0);
        public (int x, int y) Cell_Prev      { get; private set; } = (0, 0);
        public bool           IgnorePrevCell { get; private set; } = false;

        public CellPositionEvent Set(
            Int64          _entity_id, 
            int            _faction,
            (int x, int y) _cell_cur,
            (int x, int y) _cell_prev,
            bool           _ignore_prev_cell)
        {
            EntityID       = _entity_id;
            Faction        = _faction;
            Cell_Cur       = _cell_cur;
            Cell_Prev      = _cell_prev;
            IgnorePrevCell = _ignore_prev_cell;

            return this;
        }

        public void Reset()
        {
            EntityID       = 0;
            Faction        = 0;
            Cell_Cur       = (0, 0);
            Cell_Prev      = (0, 0);
            IgnorePrevCell = false;            
        }

        public void Release()
        {
            ObjectPool<CellPositionEvent>.Return(this);
        }
    }
}

