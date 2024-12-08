using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    [EventReceiverAttribute(typeof(SituationUpdatedEvent))]
    public partial class Entity : IOwner, IFaction, ICommand, IEventReceiver, IPathOwner
    {
        public Int64                ID              { get; private set; }

        public (int x, int y)       Cell            { get; private set; }
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
        // public CommandManager       CommandManager  { get; private set; }    


        public bool    IsDead       => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;



        protected Entity(Int64 _id)
        {
            ID              = _id;
            // Target          = null;
            Cell            = (0, 0);
                 
            BlackBoard      = new EntityBlackBoard();
            Skill           = new BattleSkill();
            StatusManager   = new BattleStatusManager(this);
            PathNodeManager = new PathNodeManager();
            PathVehicle     = new PathVehicle_Basic();
            Inventory       = new Inventory();
            AIManager   = new AIManager();
            // CommandManager  = new CommandManager();
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
            // CommandManager.Initialize(this);
            // PathNodeManager.Setup(CellPosition);
            PathVehicle.Setup(Cell.CellToPosition());
            
            EventDispatchManager.Instance.AttachReceiver(this);
        }

        public void Reset()
        {
            EventDispatchManager.Instance.DetachReceiver(this);
        }

        public void Update(float _delta_time)
        {
        }



        public void ApplyDamage(int _damage/*, bool _is_plan = false*/)
        {
            if (_damage <= 0)
                return;

            
            var cur_hp = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
            var new_hp = Math.Max(0, cur_hp - _damage);

            StatusManager.Status.SetPoint(EnumUnitPoint.HP, new_hp);

            Debug.Log($"GetDamaged, ID:{ID}, HP:{new_hp}");
        }

        public int GetFaction()
        {
            return BlackBoard.GetValue(EnumEntityBlackBoard.Faction);
        }

        public void SetFaction(int _faction)
        {
            BlackBoard.SetValue(EnumEntityBlackBoard.Faction, _faction);
        }


        public void OnReceiveEvent(IEventParam _param)
        {
            if (_param is SituationUpdatedEvent situation_updated)
            {
                switch(situation_updated.Situation)
                {
                    case EnumSituationType.BattleSystem_Faction_Changed: 
                    OnReceiveEvent_Faction_Changed(situation_updated); 
                    break;

                    case EnumSituationType.BattleSystem_Command_Dispatch_AI_Update:
                    OnReceiveEvent_Command_Dispatch_AI_Update(situation_updated);
                    break;
                }


                // situation 변경시 스킬 사용.
                Skill.UseSkill(situation_updated.Situation, this);


            }
        }


        void OnReceiveEvent_Faction_Changed(SituationUpdatedEvent _param)
        {
            // _param.SystemParam as Battlesys
            var system = BattleSystemManager.Instance.GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            if (system == null)
                return;

            // 턴이 변경되었을 때.
            if (system.Turn_Prev != system.Turn_Cur)
            {
                // 행동 완료 처리 reset 
                ResetCommandProgressState();
            }
        }

        
    }


}

