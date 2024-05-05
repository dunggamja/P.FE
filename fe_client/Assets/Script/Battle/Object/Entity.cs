using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [EventReceiverAttribute(typeof(SituationUpdatedEvent))]
    public partial class Entity : IOwner, IFaction, ICommand, IEventReceiver, IPathOwner
    {
        public Int64          ID       { get; private set; }
        public ITarget        Target   { get; }
        public (int x, int y) Cell     { get; private set; }


        public IBlackBoard          BlackBoard      { get; }
        public ISkill               Skill           { get; } 
        public BattleStatusManager  StatusManager   { get; } 
        public IPathNodeManager     PathNodeManager { get; } 
        public PathVehicle          PathVehicle     { get; } 


        public bool IsDead => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;


        protected Entity(Int64 _id)
        {
            ID              = _id;
            
            BlackBoard      = new BattleBlackBoard();
            Skill           = new BattleSkill();
            StatusManager   = new BattleStatusManager(this);
            PathNodeManager = new PathNodeManager();
            PathVehicle     = new PathVehicle_Basic();
        }

        public static Entity Create()
        {
            var battle_object = new Entity(Util.GenerateID());
            battle_object.Init();

            return battle_object;
        }

        public void Init()
        {
            EventDispatchManager.Instance.AttachReceiver(this);
        }

        public void Reset()
        {
            EventDispatchManager.Instance.DetachReceiver(this);
        }

        public void Update(float _delta_time)
        {
            if (PathNodeManager != null)
                PathNodeManager.Update();

            if (PathVehicle != null)
                PathVehicle.Update(this, _delta_time);
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
            return BlackBoard.GetValue(EnumBlackBoard.Faction);
        }

        public void SetFaction(int _faction)
        {
            BlackBoard.SetValue(EnumBlackBoard.Faction, _faction);
        }

        public EnumCommandType GetCommandType()
        {
            return (EnumCommandType)BlackBoard.GetValue(EnumBlackBoard.CommandType);
        }

        public void SetCommandType(EnumCommandType _command_type)
        {
            BlackBoard.SetValue(EnumBlackBoard.CommandType, (int)_command_type);
        }

        public EnumCommandState GetCommandState()
        {
            return (EnumCommandState)BlackBoard.GetValue(EnumBlackBoard.CommandState);

        }

        public void SetCommandState(EnumCommandState _command_state)
        {
            BlackBoard.SetValue(EnumBlackBoard.CommandState, (int)_command_state);
        }

        


        public void OnReceiveEvent(IEventParam _param)
        {
            if (_param is SituationUpdatedEvent situation_updated)
            {
                Skill.UseSkill(situation_updated.Situation, this);
            }
        }
    }


}

