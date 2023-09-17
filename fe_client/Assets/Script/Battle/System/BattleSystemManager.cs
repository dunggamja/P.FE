using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleSystemManager : Singleton<BattleSystemManager>, ISystemManager
    {
        Dictionary<int, BattleSystem> m_repository = new Dictionary<int, BattleSystem>();

        public IBattleSystemParam  Param      { get; private set; }
        public EnumState           State      { get; private set; }


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;


        protected override void Init()
        {
            base.Init();

            // TODO: 
            var turn_system = new BattleSystem_Turn(); m_repository.Add((int)turn_system.SystemType, turn_system);
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
            // 현재 턴/ 현재 행동하는 진영
            UpdateSystem(EnumSystem.BattleSystem_Turn, Param);

            // 유닛들 행동 update (이동, 전투, 상호작용)
            UpdateSystem(EnumSystem.BattleSystem_Command, Param);

            // 
            UpdateSystem(EnumSystem.BattleSystem_Navigation, Param);

            return false;
        }


        void OnExit()
        {
        }


        public void Update()
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

        public ISystem GetSystem(EnumSystem _system_type)
        {
            if (m_repository.TryGetValue((int)_system_type, out var system))
                return system;

            Debug.LogError($"Can't Find System, {_system_type.ToString()} in SystemManager[{GetType().ToString()}]");
            return null;
        }
        private EnumState UpdateSystem(EnumSystem _system_type, IBattleSystemParam _param)
        {
            var system = GetSystem(_system_type) as BattleSystem;
            if (system != null)
                return system.Update(_param);

            return EnumState.None;
        }

        private EnumState GetSystemState(EnumSystem _system_type)
        {
            var system = GetSystem(_system_type);
            if (system != null)
                return system.State;

            return EnumState.None;
        }

        private bool IsSystemFinished(EnumSystem _system_type)
        {
            return GetSystemState(_system_type) == EnumState.Finished;
        }


    }
}
