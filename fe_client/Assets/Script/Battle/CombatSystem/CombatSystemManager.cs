using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    
    /// <summary>
    /// 전투 씬 관리용
    /// </summary>
    public class CombatSystemManager : Singleton<CombatSystemManager>, ISystemManager
    {
        Dictionary<int, CombatSystem> m_repository = new Dictionary<int, CombatSystem>();

        public ICombatSystemParam  Param      { get; private set; }
        public EnumState           State      { get; private set; }


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;


        private bool                      m_is_combat_logic_finished = false;
        private bool                      m_is_post_process_finished = false;
        private List<Combat_DamageResult> m_list_damage_result              = new List<Combat_DamageResult>();


        protected override void Init()
        {
            base.Init();

            // 현재는 파엠을 의식하고 1대1 전투를 상정하고 만들어져 있음.
           

            var turn_sytem         = new CombatSystem_Turn();        
            var damage_sytem       = new CombatSystem_Damage();      
            var post_process_sytem = new CombatSystem_PostProcess(); 
            // var effect_sytem = new CombatSystem_Effect(); m_repository.Add((int)effect_sytem.SystemType, effect_sytem);

            m_repository.Add((int)turn_sytem.SystemType, turn_sytem);
            m_repository.Add((int)damage_sytem.SystemType, damage_sytem);
            m_repository.Add((int)post_process_sytem.SystemType, post_process_sytem);

            foreach (var e in m_repository.Values)
                e.Init();
        }


        public void Release()
        {
            Param = null;
            State = EnumState.None;


            m_is_combat_logic_finished = false;
            m_is_post_process_finished = false;
            m_list_damage_result.Clear();


            foreach (var e in m_repository.Values)
                e.Release();
        }

        public void Setup(ICombatSystemParam _param)
        {
            Param = _param;
            State = EnumState.None;

            foreach (var e in m_repository.Values)
                e.Init();
        }

        void OnEnter()
        {
            m_is_combat_logic_finished = false;
            m_is_post_process_finished = false;
            m_list_damage_result.Clear();
            // Debug.LogWarning("list_damage_result.Clear()");
        }

        bool OnUpdate()
        {
            OnUpdate_Combat_Logic();
            OnUpdate_PostProcess();

            return m_is_combat_logic_finished && m_is_post_process_finished;
        }


        void OnExit()
        {
            //Debug.Log("Battle Finished");
            foreach (var e in m_repository.Values)
                e.Release();
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


        public void AddCombatDamageResult(Combat_DamageResult _damage)
        {
            m_list_damage_result.Add(_damage);
        }

        public List<Combat_DamageResult> GetCombatDamageResult()
        {
            return m_list_damage_result;
        }

        public bool IsEngaged(Int64 _id)  => IsAttacker(_id) || IsDefender(_id);
        public bool IsAttacker(Int64 _id) => (Param != null && Param.Attacker != null && Param.Attacker.ID == _id) && 0 < _id;
        public bool IsDefender(Int64 _id) => (Param != null && Param.Defender != null && Param.Defender.ID == _id) && 0 < _id;



        bool Check_Dead()
        {
            if (Param == null)
                return false;

            if (Param.Attacker == null || Param.Defender == null)
                return false;

            // 공격자/방어자 둘 중 하나라도 죽었으면 죽음 체크 완료.
            return (Param.Attacker.IsDead || Param.Defender.IsDead);
        }


        void OnUpdate_Combat_Logic()
        {
            if (m_is_combat_logic_finished)
                return;


            // 공격자/방어자 턴 순서 계산.
            if (UpdateSystem(EnumSystem.CombatSystem_Turn, Param) == EnumState.Finished)
            {
                // 전투로직 종료 
                m_is_combat_logic_finished = true;
                return;
            }

            // 공격/방어 데미지 처리
            UpdateSystem(EnumSystem.CombatSystem_Damage, Param);

            // 실제 전투일때. 처리하는 로직.
            if (Param.IsPlan == false)
            {
                // 죽음 체크
                if (Check_Dead())
                {
                    // 전투 로직 종료.
                    m_is_combat_logic_finished = true;
                    return;
                }
            }            
        }

        void OnUpdate_PostProcess()
        {
            if (m_is_combat_logic_finished == false)
                return;

            if (m_is_post_process_finished)
                return;

            if (UpdateSystem(EnumSystem.CombatSystem_PostProcess, Param) == EnumState.Finished)
            {
                m_is_post_process_finished = true;
            }
        }
    }
}