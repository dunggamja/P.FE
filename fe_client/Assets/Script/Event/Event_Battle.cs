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

        public int                    Faction      { get; private set; } = 0;
        public EnumCommandPriority    Priority     { get; private set; } = EnumCommandPriority.None;


        public float                  Out_Score    { get; private set; } = 0;
        public Int64                  Out_EntityID { get; private set; } = 0;

        

        public void TrySetScore(Int64 _entity_id, float _score)
        {
            if (Out_Score < _score)
            {
                Out_Score    = _score;
                Out_EntityID = _entity_id;
            }
        }

        public Battle_AI_Command_DecisionEvent Set(int _faction, EnumCommandPriority _priority)
        {
            Faction  = _faction;
            Priority = _priority;
            return this;
        }

        public void Reset()
        {
            Faction           = 0;
            Priority          = EnumCommandPriority.None;
            Out_Score          = 0;
            Out_EntityID = 0;
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

        public Int64               EntityID        { get; private set; } = 0;
        public int                 Faction         { get; private set; } = 0;
        public (int x, int y)      Cell            { get; private set; } = (0, 0);
        public bool                IsEnter         { get; private set; } = false;
        

        public Battle_Cell_OccupyEvent Set(
            Int64               _entity_id, 
            int                 _faction,
            (int x, int y)      _cell,
            bool                _is_enter)
        {
            EntityID        = _entity_id;
            Faction         = _faction;
            Cell            = _cell;
            IsEnter         = _is_enter;
            return this;
        }

        public void Reset()
        {
            EntityID        = 0;
            Faction         = 0;
            Cell            = (0, 0);  
            IsEnter         = false;
        }

        public void Release()
        {
            ObjectPool<Battle_Cell_OccupyEvent>.Return(this);
        }
    }



}

