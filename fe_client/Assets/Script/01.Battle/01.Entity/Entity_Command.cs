using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    public partial class Entity 
    {
        // public void ResetAIScore()
        // {
        //     BlackBoard.Score_Attack.Reset();

        //     for(int i = AISCORE_INDEX_BEGIN ; i < AISCORE_INDEX_END ; ++i)
        //     {
        //         var ai_type = (EnumEntityBlackBoard)i;
        //         BlackBoard.SetBPValue(ai_type, 0f);
        //     }
        // }
       

        // public (EnumEntityBlackBoard _type, float score) GetAIScoreMax()
        // {
        //     var top_score_type = EnumEntityBlackBoard.None;
        //     var top_score      = 0f;

        //     for(int i = AISCORE_INDEX_BEGIN ; i < AISCORE_INDEX_END ; ++i)
        //     {
        //         var ai_type = (EnumEntityBlackBoard)i;
        //         var score = BlackBoard.GetBPValueAsFloat(ai_type);
        //         if (score > top_score)
        //         {
        //             top_score      = score;
        //             top_score_type = ai_type;
        //         }
        //     }
        //     return (top_score_type, top_score);
        // }


        

        

        public bool HasCommandEnable()
        {
            // 실행 가능한 명령이 1개라도 있는지 탐색.
            foreach(var e in Util.CachedEnumValues<EnumCommandFlag>())
            {
                if (HasCommandEnable(e))
                    return true;
            }

            return false;
            // return BlackBoard.HasValue(EnumEntityBlackBoard.CommandFlag);
        }

        public bool HasAnyCommandDone()
        {
            // 커맨드를 한개라도 진행했는지 체크.
            return BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag) != -1;
        }

        public bool HasAnyCommandDone_Without(EnumCommandFlag _command_flag)
        {
            // 매개변수로 받은 커맨드를 제외하고, 한개라도 진행했는지 체크.
            foreach(var e in Util.CachedEnumValues<EnumCommandFlag>())
            {
                if (e == _command_flag)
                    continue;

                if (HasCommandEnable(e) == false)
                    return true;
            }
            return false;
        }


        public bool HasCommandEnable(EnumCommandFlag _command_flag)
        {
            return BlackBoard.HasBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
        }

        public void SetCommandDone(EnumCommandFlag _command_flag)
        {
            BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);

            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable: {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }

        public void SetCommandEnable(EnumCommandFlag _command_flag)
        {
            BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);

            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable:  {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }


        public void ProcessMovedDistance(bool _accumulate_move_distance)
        {
            if (_accumulate_move_distance)
            {
                // 이번 행동시 이동한 거리.
                var moved_distance = BlackBoard.GetValue(EnumEntityBlackBoard.MovedDistance_Action);

                // 이번턴에 이동한 거리에 누적.
                BlackBoard.IncreaseValue(EnumEntityBlackBoard.MovedDistance_Turn, moved_distance);
            }

            // 이번 행동시 이동한 거리 초기화.
            BlackBoard.SetValue(EnumEntityBlackBoard.MovedDistance_Action, 0);

        }

        public void TryCommand_MoveAgain(bool _is_command_done)
        {
            // 이동 거리 처리.        
            ProcessMovedDistance(_is_command_done);

            // 재이동 가능한지 체크. 
            // - 이동 명령 취소의 경우.
            // - 탑승 상태이고, 이동 거리가 남아있는 경우.
            var is_moveable = (_is_command_done == false) || (PathMounted && PathMoveRange > 0);
            if (is_moveable)
            {
                SetCommandEnable(EnumCommandFlag.Move);
            }
        }


        public void SetAllCommandEnable()
        {            
            // 모든 커맨드 실행 가능 처리. (모든 비트를 1로 설정)
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, -1);

            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable:  {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }

        public void SetAllCommandDone()
        {
            // 모든 커맨드 완료 처리.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, 0);


            // 행동 종료시 초기화하는 변수.
            BlackBoard.SetValue(EnumEntityBlackBoard.MovedDistance_Turn, 0);
            BlackBoard.SetValue(EnumEntityBlackBoard.MovedDistance_Action, 0);
            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable:  {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }



        public EnumCommandProgressState GetCommandProgressState()
        {
            // 활성화 상태 체크.
            if (IsActive == false)
                return EnumCommandProgressState.Invalid;


            // 실행 가능한 명령이 1개도 없으면 완료 처리.
            if (HasCommandEnable() == false)
                return EnumCommandProgressState.Done;

            // 실행한 명령이 1개라도 있으면, 명령을 진행중인 상태.
            if (HasAnyCommandDone())
                return EnumCommandProgressState.Progress;

            // 실행한 명령이 1개도 없으면 명령 대기중인 상태.
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

        public (int is_progress, int priority) GetCommandPriorityForCompare()
        {
            return (
                // 행동 진행상태.
                (GetCommandProgressState() == EnumCommandProgressState.Progress) ? 1 : 0
                // 행동 우선순위.
                , GetCommandPriority());
        }

        public int GetCommandPriority()
        {
            // 행동 우선순위 값. 높을 수록 먼저 행동합니다. (최소값 0)
            return (int)BlackBoard.GetValue(EnumEntityBlackBoard.CommandPriority);
        }

        public void SetCommandPriority(int _priority)
        {
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandPriority, _priority);
        }

        // public EnumCommandPriority GetCommandPriority()
        // {
        //     if (IsEnableCommandProgress() == false)
        //         return EnumCommandPriority.None;
        //     if (GetCommandProgressState() == EnumCommandProgressState.Progress)
        //         return EnumCommandPriority.Critical;
        //     // TODO: �ൿ �켱����...;
        //     return EnumCommandPriority.Normal;
        // }
        // public bool ShouldSetCommandDone()
        // {
        //     // ���� ���� �ൿ�� ������ �Ϸ�ó�� �� �ʿ� ����.
        //     if (!BlackBoard.HasValue(EnumEntityBlackBoard.CommandFlag))            
        //         return false;
        //     // �̹� �Ϸ� ���¸� �Ϸ�ó�� �� �ʿ� ����.  
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
