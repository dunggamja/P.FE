using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        public EnumCommandOwner GetCommandOwner()
        {
            return (EnumCommandOwner)BlackBoard.GetValue(EnumBlackBoard.CommandOwner);
        }

        public void SetCommandOwner(EnumCommandOwner _command_owner)
        {
            BlackBoard.SetValue(EnumBlackBoard.CommandOwner, (int)_command_owner);
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

        public void ResetCommandProgressState()
        {
            BlackBoard.SetValue(EnumBlackBoard.CommandFlag, 0);
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
    }
}

public class Entity_Command : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
