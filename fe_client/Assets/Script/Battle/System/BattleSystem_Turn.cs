using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Turn : BattleSystem
    {
        // 진영별 턴  관리. 
        int   m_turn_update_count = 0;           // turn system update
        int[] m_queue_turn        = new int[2];  //[0] : Prev, [1] : Current
        int[] m_queue_faction     = new int[2];  //[0] : Prev, [1] : Current


        // 
        public int  Turn_Prev { get => m_queue_turn[0]; set => m_queue_turn[0] = value; }
        public int  Turn_Cur  { get => m_queue_turn[1]; set => m_queue_turn[1] = value; }
                    
        public int  Faction_Prev  { get => m_queue_faction[0]; set => m_queue_faction[0] = value; }
        public int  Faction_Cur   { get => m_queue_faction[1]; set => m_queue_faction[1] = value; }

        public bool IsTurnChanged    => m_queue_turn[0]    != m_queue_turn[1];
        public bool IsFactionChanged => m_queue_faction[0] != m_queue_faction[1];


        public BattleSystem_Turn() : base(EnumSystem.BattleSystem_Turn)
        { }

        public override void Init()
        {
            Release();
        }

        public override void Release()
        {
            m_turn_update_count = 0;
            Array.Clear(m_queue_turn, 0, m_queue_turn.Length);
            Array.Clear(m_queue_faction, 0, m_queue_faction.Length);
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
           //EventDispatchManager.Instance.DispatchEvent(new SituationUpdatedEvent(EnumSituationType.BattleSystem_Turn_Changed, _param));

            if (Turn_Cur == 0 && Faction_Cur == 0)
            {
                // 턴이 셋팅되지 않았을 때 초기화 처리.
                UpdateTurnAndFaction(1, 0);
            }
        }


        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            ++m_turn_update_count;

            // 현재 진영의 행동이 완료되었는지 체크.
            if (Check_Faction_Turn_Done(Faction_Cur))
            {      
                // 턴, 진영 변경 처리.
                var turn    = Turn_Cur;
                var faction = Faction_Cur;

                (var success, var next_faction) = FindNextFaction(faction);
                if  (success)
                {
                    // 다음에 행동할 진영을 셋팅.
                    faction = next_faction;
                }
                else
                {
                    // 모든 진영의 행동이 종료 되었으면 턴 증가.
                    ++turn;

                    // 진영 초기화
                    faction = 0;
                }


                UpdateTurnAndFaction(turn, faction);
            }


            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {


        }


        (bool result, int next_faction)  FindNextFaction(int _current_faction)
        {
            var result       = false;
            var next_faction = int.MaxValue;

            // 다음에 행동할 Faction을 찾습니다.  
            // TODO: 일단 모든 Entity 순회... ㅠㅠ
            EntityManager.Instance.Loop((e) =>
            {
                var faction = e.GetFaction();
                if (faction <= 0)
                    return;

                if (_current_faction < faction && faction < next_faction)
                {
                    result       = true;
                    next_faction = faction;
                }
            });

            return (result, next_faction);
        }


        bool Check_Faction_Turn_Done(int _current_faction)
        {
            bool Verify_Entity_Turn_Progress(Entity _entity)
            {
                // 진영 체크.
                if (_entity.GetFaction() != _current_faction)
                    return false;

                // 행동 완료 체크.
                if (_entity.IsEnableCommandProgress() == false)
                    return false;

                return true;
            }

            // 진영내의 유닛 중 행동이 가능한 유닛이 있다면 아직 턴 종료시점이 아닌 것.
            var active_unit  = EntityManager.Instance.Find(Verify_Entity_Turn_Progress);
            if (active_unit != null)
                return false;

            // 모든 유닛이 행동을 완료했다.
            return true;
        }


        void UpdateTurnAndFaction(int _turn, int _faction)
        {
            Turn_Prev    = Turn_Cur;
            Faction_Prev = Faction_Cur;

            Turn_Cur     = _turn;
            Faction_Cur  = _faction;        


            // BlackBoard에 셋팅.
            BattleSystemManager.Instance.BlackBoard.SetValue(EnumBattleBlackBoard.TurnUpdateCount, m_turn_update_count);
            BattleSystemManager.Instance.BlackBoard.SetValue(EnumBattleBlackBoard.CurrentTurn,     Turn_Cur);
            BattleSystemManager.Instance.BlackBoard.SetValue(EnumBattleBlackBoard.CurrentFaction,  Faction_Cur);

            // 이벤트 디스팻치.
            if (Faction_Cur != Faction_Prev) 
            {                  
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Situation_UpdateEvent>
                    .Acquire()
                    .Set(EnumSituationType.BattleSystem_Faction_Changed)//, false)
                    );
            }

            if (Turn_Cur != Turn_Prev) 
            {
                EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Situation_UpdateEvent>
                    .Acquire()
                    .Set(EnumSituationType.BattleSystem_Turn_Changed)//, false)
                    );
            }    
        }

        
    }

}

