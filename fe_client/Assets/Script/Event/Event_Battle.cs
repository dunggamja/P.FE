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
        public bool                   IsPlan      { get; private set; }  = false;
        // public ISystemParam       SystemParam { get; private set; }   

        public Battle_Situation_UpdateEvent Set(EnumSituationType _situation_type, bool _is_plan)
        {
            Situation   = _situation_type;
            IsPlan      = _is_plan;
            // SystemParam = _system_param;
            return this;
        }

        public void Reset()
        {
            Situation = EnumSituationType.None;
            IsPlan    = false;
        }

        public void Release()
        {
            // var temp = this;
            ObjectPool<Battle_Situation_UpdateEvent>.Return(this);
        }
    }


    public class Battle_Scene_ChangeEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;
        public bool   IsEnter { get; private set; } = false;

        public void Release()
        {
            // var temp = this;
            ObjectPool<Battle_Scene_ChangeEvent>.Return(this);
        }

        public void Reset()
        {
            IsEnter = false;
        }

        public Battle_Scene_ChangeEvent Set(bool _is_enter)
        {
            IsEnter = _is_enter;
            return this;
        }
    }

    // public class Battle_AI_Command_DecisionEvent : IEventParam
    // {
    //     // AI ���� ���� �̺�Ʈ.

    //     public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;

    //     public int                    Faction      { get; private set; } = 0;
    //     public EnumCommandPriority    Priority     { get; private set; } = EnumCommandPriority.None;


    //     public float                  Out_Score    { get; private set; } = 0;
    //     public Int64                  Out_EntityID { get; private set; } = 0;

        

    //     public void TrySetScore(Int64 _entity_id, float _score)
    //     {
    //         if (Out_Score < _score)
    //         {
    //             Out_Score    = _score;
    //             Out_EntityID = _entity_id;
    //         }
    //     }

    //     public Battle_AI_Command_DecisionEvent Set(int _faction, EnumCommandPriority _priority)
    //     {
    //         Faction  = _faction;
    //         Priority = _priority;
    //         return this;
    //     }

    //     public void Reset()
    //     {
    //         Faction           = 0;
    //         Priority          = EnumCommandPriority.None;
    //         Out_Score          = 0;
    //         Out_EntityID = 0;
    //     }

    //     public void Release()
    //     {
    //         ObjectPool<Battle_AI_Command_DecisionEvent>.Return(this);
    //     }
    // }

    public class Battle_Cell_OccupyEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;

        public Int64               EntityID        { get; private set; } = 0;
        public int                 Faction         { get; private set; } = 0;
        public (int x, int y)      Cell            { get; private set; } = (0, 0);
        public bool                IsOccupy        { get; private set; } = false;  // 
        

        public Battle_Cell_OccupyEvent Set(
            Int64               _entity_id, 
            int                 _faction,
            (int x, int y)      _cell,
            bool                _is_occupy)
        {
            EntityID        = _entity_id;
            Faction         = _faction;
            Cell            = _cell;
            IsOccupy        = _is_occupy;
            return this;
        }

        public void Reset()
        {
            EntityID        = 0;
            Faction         = 0;
            Cell            = (0, 0);  
            IsOccupy         = false;
        }

        public void Release()
        {
            // var temp = this;
            ObjectPool<Battle_Cell_OccupyEvent>.Return(this);
        }
    }


    public class Battle_Entity_HP_UpdateEvent : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

        public Int64 EntityID { get; private set; } = 0;
        public int  HP        { get; private set; } = 0;

        public void Release()
        {
            // var temp = this;
            ObjectPool<Battle_Entity_HP_UpdateEvent>.Return(this);
        }

        public void Reset()
        {
            EntityID = 0;
            HP       = 0;
        }

        public Battle_Entity_HP_UpdateEvent Set(Int64 _entity_id, int _hp)
        {
            EntityID = _entity_id;
            HP       = _hp;
            return this;
        }
    }


    public class Battle_Command_Event : IEventParam
    {
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.OnNextUpdate;

        public Int64           EntityID    { get; private set; } = 0;
        public EnumCommandType CommandType { get; private set; } = EnumCommandType.None;

        public void Release()
        {
            // var temp = this;
            ObjectPool<Battle_Command_Event>.Return(this);
        }

        public void Reset()
        {
            EntityID    = 0;
            CommandType = EnumCommandType.None;
        }

        public Battle_Command_Event Set(Int64 _entity_id, EnumCommandType _command_type)
        {
            EntityID    = _entity_id;
            CommandType = _command_type;
            return this;
        }
    }


    public class Battle_Item_UpdateEvent : IEventParam
    {
 
        public EnumEventProcessTiming EventProcessTiming => EnumEventProcessTiming.Immediate;

        public Int64              EntityID   { get; private set; } = 0;
        public Int32              ItemKind   { get; private set; } = 0;
        public EnumItemActionType ActionType { get; private set; } = EnumItemActionType.None;

        

        // TODO: 디테일은 나중에 생각해보자.
        // public Int64 ItemID { get; private set; } = 0;

        public void Release()
        {
            ObjectPool<Battle_Item_UpdateEvent>.Return(this);
        }

        public void Reset()
        {
            EntityID   = 0;
            ItemKind   = 0;
            ActionType = EnumItemActionType.None;
        }

        public Battle_Item_UpdateEvent Set(Int64 _entity_id, Int32 _item_kind, EnumItemActionType _action_type)
        {
            EntityID   = _entity_id;
            ItemKind   = _item_kind;
            ActionType = _action_type;
            return this;
        }

        // public void Reset()
        // {
        //     ItemID = 0;
        // }
    }
}

