﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class BattleSystem_Turn : BattleSystem
    {
        //
        int[] m_queue_turn    = new int[2];
        int[] m_queue_faction = new int[2];

        // 
        public int  Turn_Prev        => m_queue_turn[0];
        public int  Turn_Cur         => m_queue_turn[1];
                    
        public int  Faction_Prev     => m_queue_faction[0];
        public int  Faction_Cur      => m_queue_faction[1];

        public bool IsTurnChanged    => m_queue_turn[0]    != m_queue_turn[1];
        public bool IsFactionChanged => m_queue_faction[0] != m_queue_faction[1];


        public BattleSystem_Turn() : base(EnumSystem.BattleSystem_Turn)
        { }

        public override void Reset()
        {
            Array.Clear(m_queue_turn, 0, m_queue_turn.Length);
            Array.Clear(m_queue_faction, 0, m_queue_faction.Length);
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            Reset();

            // 1턴, 진영은 미정.
            UpdateTurnAndFaction(1, 0);

            // 이벤트.
            EventManager.Instance.DispatchEvent(new EventParam(this));
        }


        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // 다음에 행동할 진영을 찾아야 하는지 체크.
            var turn    = Turn_Cur;
            var faction = Faction_Cur;

            if (Check_Faction_Complete(faction))
            {
                (var result, var next_faction) = FindNextFaction(faction);
                if  (result)
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
            }


            UpdateTurnAndFaction(turn, faction);

            EventManager.Instance.DispatchEvent(new EventParam(this));
            return false;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {

        }


        (bool result, int next_faction)  FindNextFaction(int _current_faction)
        {
            var result       = false;
            var next_faction = int.MaxValue;

            // 다음에 행동할 Faction을 찾습니다.  
            // TODO: 모든 BattleObject 를 순회하도록 해두었슴...;  혹시 성능에 문제가 된다면 나중에 개선....
            BattleObjectManager.Instance.Loop((e) =>
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


        bool Check_Faction_Complete(int _current_faction)
        {
            // 진영내의 유닛 중 행동을 하지 않은 유닛이 있는지 체크.
            var active_unit = BattleObjectManager.Instance.Find(e => e.GetFaction() == _current_faction && e.GetCommandState() == EnumCommandState.Active);
            if (active_unit != null)
                return false;

            // 모든 유닛이 행동을 완료했다.
            return true;
        }


        void UpdateTurnAndFaction(int _turn, int _faction)
        {
            m_queue_turn[0]    = m_queue_turn[1];
            m_queue_faction[0] = m_queue_faction[1];

            m_queue_turn[1]    = _turn;
            m_queue_faction[1] = _faction;
        }
    }

}

