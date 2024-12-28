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
            // �˻��� AIScore ���
            var list_ai = new [] 
            { 
                EnumEntityBlackBoard.AIScore_Attack
              , EnumEntityBlackBoard.AIScore_Done 
            };

            // ���� ���� Score�� ��ȯ�մϴ�.
            return list_ai.Aggregate((type:EnumEntityBlackBoard.None, score:0f), (top_score, e) =>
            {
                var     score = BlackBoard.GetBPValueAsFloat(e);
                return (score > top_score.score) ? (e, score) : top_score;
            });
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

        public void SetCommandDone(params EnumCommandFlag[] _command_flag)
        {
            // if (_set_flag) BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag,   (byte)_command_flag);
            // else           BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);

            foreach (var e in _command_flag)
                BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)e);
        }

        public void SetCommandEnable(params EnumCommandFlag[] _command_flag)
        {
            // if (_set_flag) BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag,   (byte)_command_flag);
            // else           BlackBoard.ResetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)_command_flag);
            foreach (var e in _command_flag)
                BlackBoard.SetBitFlag(EnumEntityBlackBoard.CommandFlag, (byte)e);
        }


        public void SetAllCommandEnable()
        {            
            // ��� ��Ʈ�÷��׸� 1�� ���ش�.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, -1);
        }

        public void SetAllCommandDone()
        {
            // ��� ��Ʈ�÷��׸� 0���� ���ش�.
            BlackBoard.SetValue(EnumEntityBlackBoard.CommandFlag, 0);
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

        public bool IsEnableCommandProgress(int _faction)
        {
            // TODO: ���� üũ�ϴ� �κ� ��������...
            //       LOOP ���鼭 ã�� ���ؼ� ����� ���µ� ����ȭ �ϸ� �ʿ���� �ڵ���.
            // ������ �ٸ��� �ൿ �Ұ���.
            if (GetFaction() != _faction)
                return false;

            return IsEnableCommandProgress();
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
