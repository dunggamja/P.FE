using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        // public EnumCommandOwner GetCommandOwner()
        // {
        //     return (EnumCommandOwner)BlackBoard.GetValue(EnumEntityBlackBoard.CommandOwner);
        // }

        // public void SetCommandOwner(EnumCommandOwner _command_owner)
        // {
        //     BlackBoard.SetValue(EnumEntityBlackBoard.CommandOwner, (int)_command_owner);
        // }

        public (EnumEntityBlackBoard _type, float score) GetAIScoreMax()
        {
            // 검사할 AIScore 목록
            Span<EnumEntityBlackBoard> list_ai_type = stackalloc EnumEntityBlackBoard[2] 
            { 
                EnumEntityBlackBoard.AIScore_Attack
              , EnumEntityBlackBoard.AIScore_Done 
            };


            var top_score_type = EnumEntityBlackBoard.None;
            var top_score      = 0f;

            // 가장 높은 Score를 반환합니다.
            foreach (var ai_type in list_ai_type)
            {
                var score = BlackBoard.GetBPValueAsFloat(ai_type);
                if (score > top_score)
                {
                    top_score      = score;
                    top_score_type = ai_type;
                }
            }

            return (top_score_type, top_score);
        }

        public bool HasCommandEnable()
        {
            // 가능한 행동이 있는지 체크.
            foreach(var e in Util.CachedEnumValues<EnumCommandFlag>())
            {
                if (HasCommandEnable(e))
                    return true;
            }

            return false;
            // return BlackBoard.HasValue(EnumEntityBlackBoard.CommandFlag);
        }

        public bool HasCommandDone()
        {
            // 진행한 행동이 있는지 체크.
            foreach(var e in Util.CachedEnumValues<EnumCommandFlag>())
            {
                if (!HasCommandEnable(e))
                    return true;                
            }

            // 진행한 행동이 없는 상태.
            return false;
        }


        public bool HasCommandEnable(EnumCommandFlag _command_flag)
        {
            return BlackBoard.HasBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public void SetCommandDone(EnumCommandFlag _command_flag)
        {
            // if (_set_flag) BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag,   (byte)_command_flag);
            // else           BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);

            // foreach (var e in )
                BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public void SetCommandEnable(EnumCommandFlag _command_flag)
        {
            // if (_set_flag) BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag,   (byte)_command_flag);
            // else           BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
            // foreach (var e in _command_flag)
                BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
        }


        public void SetAllCommandEnable()
        {            
            // 모든 비트플래그를 1로 해준다.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, -1);
        }

        public void SetAllCommandDone()
        {
            // 모든 비트플래그를 0으로 해준다.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, 0);
        }


        public EnumCommandProgressState GetCommandProgressState()
        {
            // 죽었다.
            if (IsDead)
                return EnumCommandProgressState.Invalid;

            // 가능한 행동이 없다.
            if (!HasCommandEnable())
                return EnumCommandProgressState.Done;

            // 진행한 행동이 있음.
            if (HasCommandDone())
                return EnumCommandProgressState.Progress;

            // 대기 상태.
            return EnumCommandProgressState.None;
        }

        public bool IsEnableCommandProgress()
        {
            var progress_state = GetCommandProgressState();
            if (progress_state == EnumCommandProgressState.None
            ||  progress_state == EnumCommandProgressState.Progress)
                return true;

            return false;
        }

        public bool IsEnableCommandProgress(int _faction)
        {
            // TODO: 진영 체크하는 부분 삭제하자...
            //       LOOP 돌면서 찾기 위해서 만들어 놨는데 최적화 하면 필요없을 코드임.
            // 진영이 다르면 행동 불가능.
            if (GetFaction() != _faction)
                return false;

            return IsEnableCommandProgress();
        }

        


        // public bool ShouldSetCommandDone()
        // {
        //     // 진행 중인 행동이 없으면 완료처리 할 필요 없음.
        //     if (!BlackBoard.HasValue(EnumEntityBlackBoard.CommandFlag))            
        //         return false;

        //     // 이미 완료 상태면 완료처리 할 필요 없음.  
        //     if (HasCommandFlag(EnumCommandFlag.Done))
        //         return false;
            

        //     return true;
        // }

        
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
