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
        public IBlackBoard          BlackBoard      { get; }
        public ISkill               Skill           { get; } 
        public BattleStatusManager  StatusManager   { get; } 
        public Inventory            Inventory       { get; }
        public IPathNodeManager     PathNodeManager { get; } 
        public PathVehicle          PathVehicle     { get; } 
        public int                  PathAttribute   { get; private set; }
        public SensorManager        SensorManager   { get; private set; }
        public CommandManager       CommandManager  { get; private set; }    


        public bool IsDead => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;


        protected Entity(Int64 _id)
        {
            ID              = _id;
            // Target          = null;
            Cell            = (0, 0);
                 
            BlackBoard      = new BattleBlackBoard();
            Skill           = new BattleSkill();
            StatusManager   = new BattleStatusManager(this);
            PathNodeManager = new PathNodeManager();
            PathVehicle     = new PathVehicle_Basic();
            Inventory       = new Inventory();
            SensorManager   = new SensorManager();
            CommandManager  = new CommandManager();
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
            SensorManager.Initialize(this);
            CommandManager.Initialize(this);


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

        public bool HasCommandFlag(EnumCommandFlag _command_flag)
        {
            return BlackBoard.HasBitFlag(EnumBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public void SetCommandFlag(EnumCommandFlag _command_flag, bool _set_flag)
        {
            if (_set_flag) BlackBoard.SetBitFlag(EnumBlackBoard.CommandFlag,   (byte)_command_flag);
            else           BlackBoard.ResetBitFlag(EnumBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public EnumCommandProgressState GetCommandProgressState(int _faction)
        {
            // 진영이 다르면 행동 불가능.
            if (GetFaction() != _faction)
                return EnumCommandProgressState.Invalid;

            // 행동 완료 상태.
            if (HasCommandFlag(EnumCommandFlag.Done))
                return EnumCommandProgressState.Done;

            // 그 외 값이 있으면 진행중인 행동이 있음.
            if (BlackBoard.HasValue(EnumBlackBoard.CommandFlag))            
                return EnumCommandProgressState.Progress;

            // 대기 상태.
            return EnumCommandProgressState.None;
        }

        public bool IsEnableCommandProgress(int _faction)
        {
            var progress_state = GetCommandProgressState(_faction);
            if (progress_state == EnumCommandProgressState.None
            ||  progress_state == EnumCommandProgressState.Progress)
                return true;

            return false;
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

