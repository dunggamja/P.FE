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
            return (EnumCommandOwner)BlackBoard.GetValue(EnumEntityBlackBoard.CommandOwner);
        }

        public void SetCommandOwner(EnumCommandOwner _command_owner)
        {
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandOwner, (int)_command_owner);
        }

        public bool HasCommandFlag(EnumCommandFlag _command_flag)
        {
            return BlackBoard.HasBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public void SetCommandFlag(EnumCommandFlag _command_flag, bool _set_flag)
        {
            if (_set_flag) BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag,   (byte)_command_flag);
            else           BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public void ResetCommandProgressState()
        {
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, 0);
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
            if (BlackBoard.HasValue(EnumEntityBlackBoard.CommandFlag))            
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


        public bool ShouldSetCommandDone()
        {
            // 진행 중인 행동이 없으면 완료처리 할 필요 없음.
            if (!BlackBoard.HasValue(EnumEntityBlackBoard.CommandFlag))            
                return false;

            // 이미 완료 상태면 완료처리 할 필요 없음.  
            if (HasCommandFlag(EnumCommandFlag.Done))
                return false;
            

            return true;
        }

        
    }
}

// public class Entity_Command : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }
