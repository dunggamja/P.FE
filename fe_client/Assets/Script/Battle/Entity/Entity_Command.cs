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
       

        public (EnumEntityBlackBoard _type, float score) GetAIScoreMax()
        {
            // �˻��� AIScore ���
            Span<EnumEntityBlackBoard> list_ai_type = stackalloc EnumEntityBlackBoard[2] 
            { 
                EnumEntityBlackBoard.AIScore_Attack
              , EnumEntityBlackBoard.AIScore_Done 
            };


            var top_score_type = EnumEntityBlackBoard.None;
            var top_score      = 0f;

            // ���� ���� Score�� ��ȯ�մϴ�.
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
            // ������ �ൿ�� �ִ��� üũ.
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
            // ������ �ൿ�� �ִ��� üũ.
            foreach(var e in Util.CachedEnumValues<EnumCommandFlag>())
            {
                if (!HasCommandEnable(e))
                    return true;                
            }

            // ������ �ൿ�� ���� ����.
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
            // ��� ��Ʈ�÷��׸� 1�� ���ش�.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, -1);

            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable:  {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }

        public void SetAllCommandDone()
        {
            // ��� ��Ʈ�÷��׸� 0���� ���ش�.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, 0);

            // var bit_value =BlackBoard.GetValue(EnumEntityBlackBoard.CommandFlag);
            // Debug.Log($"SetCommandEnable:  {ID}, {Convert.ToString(bit_value, 2).PadLeft(32, '0')}");
        }


        public EnumCommandProgressState GetCommandProgressState()
        {
            // �׾���.
            if (IsDead)
                return EnumCommandProgressState.Invalid;

            // ������ �ൿ�� ����.
            if (!HasCommandEnable())
                return EnumCommandProgressState.Done;

            // ������ �ൿ�� ����.
            if (HasCommandDone())
                return EnumCommandProgressState.Progress;

            // ��� ����.
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

        public EnumCommandPriority GetCommandPriority()
        {
            if (IsEnableCommandProgress() == false)
                return EnumCommandPriority.None;

            if (GetCommandProgressState() == EnumCommandProgressState.Progress)
                return EnumCommandPriority.Critical;

            // TODO: �ൿ �켱����...;
            return EnumCommandPriority.Normal;
        }
        


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
