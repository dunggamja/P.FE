using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleObject : IOwner, IFaction, ICommand
    {
        public Int64   ID     { get; private set; }
        public ITarget Target { get; }


        public IBlackBoard          BlackBoard    { get; }
        public ISkill               Skill         { get; }
        public BattleStatusManager  StatusManager { get; }



        public bool IsDead => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;


        protected BattleObject(Int64 _id)
        {
            ID = _id;

            BlackBoard    = new BattleBlackBoard();
            Skill         = new BattleSkill();
            StatusManager = new BattleStatusManager(this);
        }

        public static BattleObject Create()
        {
            var    battle_object = new BattleObject(Util.GenerateID());

            return battle_object;
        }


        public void ApplyDamage(int _damage/*, bool _is_plan = false*/)
        {
            if (_damage <= 0)
                return;

            // TODO: 플랜에 대한 처리를 좀 더 깔끔하게 해보자...

            var cur_hp = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
            var new_hp = Math.Max(0, cur_hp - _damage);

            StatusManager.Status.SetPoint(EnumUnitPoint.HP, new_hp);

            Debug.Log($"GetDamaged, ID:{ID}, HP:{new_hp}");
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
    }


}

