using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleSystem_Turn : BattleSystem
    {
        // 진영별 턴 관리용

        // TODO: 일단 턴 체크 등을 할 때 모든 BattleObject 를 순회하도록 해두었슴...;
        //       혹시 성능에 문제가 된다면 나중에 개선....


        public int  Turn_Current     { get; private set; } // 현재 턴
        public int  Faction_Current  { get; private set; } // 현재 움직이는 진영
       


        public BattleSystem_Turn() : base(EnumSystem.BattleSystem_Turn)
        { }

        public override void Reset()
        {
            Turn_Current    = 0;
            Faction_Current = 0;
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            
            if (Faction_Current <= 0)
            {
                (var result, var next_faction) = FindNextFaction(Faction_Current);
                if  (result)
                {
                    Faction_Current = next_faction;
                }
                else
                {
                    // 이 경우는 종료 처리한다.
                    return true;
                }
            }


            // 해당 진영의 행동이 종료되었는지 체크
            if (Check_Faction_Complete(Faction_Current))
            {
                (var result, var next_faction) = FindNextFaction(Faction_Current);
                if  (result)
                {
                    Faction_Current = next_faction;
                }
                else
                {
                    ++Turn_Current;
                    Faction_Current = 0;
                }
            }


            // 턴 매니저가 종료된다는 개념이... 
            return false;
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

            // 유닛이 행동을 완료했다면 진영의 턴이 종료됨
            return true;
        }
    }

}

