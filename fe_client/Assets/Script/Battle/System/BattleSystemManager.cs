using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    
    /// <summary>
    /// 전투 씬을 관리하기 위한 시스템 매니저입니다.
    /// </summary>
    public class BattleSystemManager : Singleton<BattleSystemManager>, ISystemManager
    {
        Dictionary<int, BattleSystem> m_repository = new Dictionary<int, BattleSystem>();

        public IBattleSystemParam  Param      { get; private set; }
        public EnumState           State      { get; private set; }


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;
        public BattleSystem_Turn   TurnSystem   => GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
        public BattleSystem_Action ActionSystem => GetSystem(EnumSystem.BattleSystem_Action) as BattleSystem_Action;


        


        protected override void Init()
        {
            base.Init();


            var turn_sytem   = new BattleSystem_Turn();   m_repository.Add((int)turn_sytem.SystemType, turn_sytem);
            var action_sytem = new BattleSystem_Action(); m_repository.Add((int)action_sytem.SystemType, action_sytem);
        }


        public void Reset()
        {
            Param = null;
            State = EnumState.None;

            foreach (var e in m_repository.Values)
                e.Reset();
        }

        public void SetData(IBattleSystemParam _param)
        {
            Param = _param;
        }

        void OnEnter()
        {
        }

        bool OnUpdate()
        {
            // 공격자/방어자 턴 정하기 
            UpdateSystem(EnumSystem.BattleSystem_Turn, Param);

            // 턴이 정해졌으면 행동을 처리.
            UpdateSystem(EnumSystem.BattleSystem_Action, Param);

            // 턴이 모두 종료되면 전투 씬 종료 처리.
            if (GetSystemState(EnumSystem.BattleSystem_Turn) == EnumState.Finished)
                return true;

            return false;
        }


        void OnExit()
        {
        }


        void Update()
        {
            if (State != EnumState.Progress)
            {
                //State = EnumState.Init;
                State = EnumState.Progress;
                OnEnter();
            }

            if (OnUpdate())
            {
                State = EnumState.Finished;
            }

            if (State != EnumState.Progress)
            {
                OnExit();
            }
        }

        public  ISystem GetSystem(EnumSystem _system_type) => m_repository.TryGetValue((int)_system_type, out var value) ? value : null;
        private void    UpdateSystem(EnumSystem _system_type, IBattleSystemParam _param)
        {
            var system  = GetSystem(_system_type) as BattleSystem;
            if (system != null)
                system.Update(_param);
        }

        private EnumState GetSystemState(EnumSystem _system_type)
        {
            var system = GetSystem(_system_type);
            if (system != null)
                return system.State;

            return EnumState.None;
        }


        public bool IsEngaged(Int64 _id)  => IsAttacker(_id) || IsDefender(_id);
        public bool IsAttacker(Int64 _id) => (Param != null && Param.Attacker != null && Param.Attacker.ID == _id) && 0 < _id;
        public bool IsDefender(Int64 _id) => (Param != null && Param.Defender != null && Param.Defender.ID == _id) && 0 < _id;

    }
}