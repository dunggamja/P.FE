using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity : IOwner, IFaction, ICommand, IEventReceiver, IPathOwner
    {
        public Int64                ID              { get; private set; }

        public (int x, int y)       Cell            { get; private set; }
        public (int x, int y)       Cell_Prev       { get; private set; }
        // public ITarget              Target          { get; }
        public EntityBlackBoard     BlackBoard      { get; }
        public ISkill               Skill           { get; } 
        public BattleStatusManager  StatusManager   { get; } 
        public Inventory            Inventory       { get; }
        public PathNodeManager      PathNodeManager { get; } 
        public PathVehicle          PathVehicle     { get; } 
        public int                  PathAttribute   { get; private set; }
        public int                  PathZOCFaction  => GetFaction();
        public AIManager            AIManager       { get; private set; }

        public CommandManager       CommandManager  { get; private set; }    

        // public bool                 IsCommandAbort { get; private set; }
        // public bool                 IsCommandDirty { get; private set; }



        public bool    IsDead       => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;

        



        protected Entity(Int64 _id)
        {
            ID              = _id;
            // Target          = null;
            Cell            = (0, 0);
            Cell_Prev       = Cell;
                 
            BlackBoard      = new EntityBlackBoard();
            Skill           = new BattleSkill();
            StatusManager   = new BattleStatusManager(this);
            PathNodeManager = new PathNodeManager();
            PathVehicle     = new PathVehicle_Basic();
            Inventory       = new Inventory();
            AIManager       = new AIManager();
            CommandManager  = new CommandManager();



            // IsCommandAbort  = false;

            // IsCommandDirty  = false;
            // CommandManager = new CommandManager();
        }

        public static Entity Create(Int64 _id)
        {
            var new_entity = new Entity(_id);
            new_entity.Initialize();

            return new_entity;
        }

        public void Initialize()
        {
            Inventory.Initialize(this);
            AIManager.Initialize(this);
            CommandManager.Initialize(this);

            EventDispatchManager.Instance.AttachReceiver(this);


            //PathNodeManager.Setup(CellPosition);
            //PathVehicle.Setup(Cell.CellToPosition());            
        }

        public void Reset()
        {
            EventDispatchManager.Instance.DetachReceiver(this);
        }




        public void ApplyDamage(int _damage/*, bool _is_plan = false*/)
        {
            if (_damage <= 0)
                return;

            
            var cur_hp = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
            var new_hp = Math.Max(0, cur_hp - _damage);

            StatusManager.Status.SetPoint(EnumUnitPoint.HP, new_hp);

            // Debug.Log($"GetDamaged, ID:{ID}, HP:{new_hp}");
        }

        public int GetFaction()
        {
            return BlackBoard.GetValue(EnumEntityBlackBoard.Faction);
        }

        public void SetFaction(int _faction)
        {
            BlackBoard.SetValue(EnumEntityBlackBoard.Faction, _faction);
        }

        // public void SetCommandDirty(bool _is_dirty)
        // {
        //     IsCommandDirty = _is_dirty;
        // }

    
   

    }


}

