using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    
    /// <summary>
    /// 전투 씬을 관리하기 위한 시스템 매니저입니다.
    /// </summary>
    public class CombatSystemManager : Singleton<CombatSystemManager>, ISystemManager
    {
        Dictionary<int, CombatSystem> m_repository = new Dictionary<int, CombatSystem>();

        public ICombatSystemParam  Param      { get; private set; }
        public EnumState           State      { get; private set; }


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;


        protected override void Init()
        {
            base.Init();


            var turn_sytem   = new CombatSystem_Turn();   m_repository.Add((int)turn_sytem.SystemType, turn_sytem);
            var damage_sytem = new CombatSystem_Damage(); m_repository.Add((int)damage_sytem.SystemType, damage_sytem);
        }


        public void Reset()
        {
            Param = null;
            State = EnumState.None;

            foreach (var e in m_repository.Values)
                e.Reset();
        }

        public void SetData(ICombatSystemParam _param)
        {
            Param = _param;
        }

        void OnEnter()
        {
        }

        bool OnUpdate()
        {
            // 공격자/방어자 턴 셋팅
            if (UpdateSystem(EnumSystem.CombatSystem_Turn, Param) == EnumState.Finished)
            {
                // 공격자/방어자 턴 셋팅 실패하면 바로 종료 처리.
                return true;
            }

            // 데미지 계산.
            UpdateSystem(EnumSystem.CombatSystem_Damage, Param);

            // 공격자/방어자 중 1명이 죽었으면 종료 처리.
            if (Param.Attacker.IsDead || Param.Defender.IsDead)
                return true;

            return false;
        }


        void OnExit()
        {
            //Debug.Log("Battle Finished");
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
        private EnumState UpdateSystem(EnumSystem _system_type, ICombatSystemParam _param)
        {
            var system  = GetSystem(_system_type) as CombatSystem;
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



        public bool IsEngaged(Int64 _id)  => IsAttacker(_id) || IsDefender(_id);
        public bool IsAttacker(Int64 _id) => (Param != null && Param.Attacker != null && Param.Attacker.ID == _id) && 0 < _id;
        public bool IsDefender(Int64 _id) => (Param != null && Param.Defender != null && Param.Defender.ID == _id) && 0 < _id;


        bool CheckIsFinished()
        {
            

            

            return false;
        }

        bool OnUpdate_Verify()
        {
            // 널 체크
            if (Param == null || Param.Attacker == null || Param.Defender == null)
                return false;

            // 둘중 1명이 죽었다면 전투씬 종료 처리.
            if (Param.Attacker.IsDead || Param.Defender.IsDead)
                return true;

            

            return true;
        }

    }
}