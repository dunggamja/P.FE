using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battle
{
    public partial class BattleSystem_Command : BattleSystem, IEventReceiver
    {
        // Î™ÖÎ†π?ùÑ Ï≤òÎ¶¨?ï¥Î≥¥Ïûê.
        // public class CommandData
        // {
        //     public Int64     UnitID     { get; private set; }
        //     public EnumState State      { get; private set; } 

        //     public void Reset()
        //     {
        //         UnitID = 0;
        //         State  = EnumState.None;
        //     }
        // }

        // Ï≤òÎ¶¨?ï† Î™ÖÎ†π ?ç∞?ù¥?Ñ∞. ?àúÏ∞®Ï†Å?úºÎ°? Ï≤òÎ¶¨?ïú?ã§.
        // Queue<CommandData> m_list_command;       


        public BattleSystem_Command() : base(EnumSystem.BattleSystem_CommandDispatch)
        {
        }



        public override void Init()
        {
            
        }

        public override void Release()
        {
            // m_list_command.Clear();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // while(m_list_command.Count > 0)
            // {
            //     if (m_list_command.TryPeek(out var command))
            //     {
            //             
            //     }
            //     else
            //     {
            //         m_list_command.Dequeue();
            //     }
            // }



            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }


        public void OnReceiveEvent(IEventParam _param)
        {
           // EventReceiver∏¶ ±ª¿Ã π´Ωº ¿Ã¿Ø∑Œ √ﬂ∞°«ﬂ¿ª±Ó???
        }
    }
}