using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Battle_Situation_UpdateEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;
        public EnumSituationType      Situation   { get; private set; }  = EnumSituationType.None;
        // public ISystemParam       SystemParam { get; private set; }   

        public Battle_Situation_UpdateEvent Set(EnumSituationType _situation_type)
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
            ObjectPool<Battle_Situation_UpdateEvent>.Return(this);
        }
    }

    public class Battle_AI_Command_DecisionEvent : IEventParam
    {
        // AI ��� ���� �̺�Ʈ.

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
            ObjectPool<Battle_AI_Command_DecisionEvent>.Return(this);
        }
    }

    public class Battle_Cell_OccupyEvent : IEventParam
    {
        // �� ���� �̺�Ʈ.

        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;

        public Int64          EntityID       { get; private set; } = 0;
        public int            Faction        { get; private set; } = 0;
        public (int x, int y) Cell_Cur       { get; private set; } = (0, 0);
        public (int x, int y) Cell_Prev      { get; private set; } = (0, 0);
        public bool           IgnorePrevCell { get; private set; } = false;

        public Battle_Cell_OccupyEvent Set(
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
            ObjectPool<Battle_Cell_OccupyEvent>.Return(this);
        }
    }
}

