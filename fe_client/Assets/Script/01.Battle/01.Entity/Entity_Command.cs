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

        public bool HasCommandDone()
        {
            // 완료한 명령이 1개라도 있는지 체크.
            foreach(var e in Util.CachedEnumValues<EnumCommandFlag>())
            {
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

            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable:  {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }

        public bool IsAnyCommandDone()
        {
            // 커맨드를 한개라도 진행했는지 체크.
            return BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag) != -1;
        }


        public EnumCommandProgressState GetCommandProgressState()
        {
            // 죽었음.
            if (IsDead)
                return EnumCommandProgressState.Invalid;

            // 실행 가능한 명령이 1개도 없으면 완료 처리.
            if (HasCommandEnable() == false)
                return EnumCommandProgressState.Done;

            // 실행한 명령이 1개라도 있으면, 명령을 진행중인 상태. <- 재이동시에는 이걸 어떻게 한다... 
            if (HasCommandDone())
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
