using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct Combat_DamageResult
    {
        public Int64  AttackerID         { get; private set; }
        public Int64  TargetID           { get; private set; }
        public Int64  WeaponID           { get; private set; }

        public int    Result_HP_Attacker { get; private set; }
        public int    Result_HP_Target   { get; private set; }
        


        public bool   WeaponEffectiveness { get; private set; }
        public bool   Result_Hit          { get; private set; }
        public bool   Result_Critical     { get; private set; }
        public bool   Result_Guard        { get; private set; }

        public float  Result_HitRate      { get; private set; }
        public float  Result_CriticalRate { get; private set; }
        public float  Result_GuardRate    { get; private set; }
        public int    Result_Damage       { get; private set; }   

        public int    Result_HitRate_Percent      => Math.Clamp((int)(Result_HitRate * 100), 0, 100);
        public int    Result_CriticalRate_Percent => Math.Clamp((int)(Result_CriticalRate * 100), 0, 100);
        public int    Result_GuardRate_Percent    => Math.Clamp((int)(Result_GuardRate * 100), 0, 100);

        public static Combat_DamageResult Create(
            Int64 _attacker_id,
            Int64 _target_id,
            Int64 _weapon_id,
            int   _result_hp_attacker,
            int   _result_hp_target,
            bool  _weapon_effectiveness, 
            bool  _result_hit,
            bool  _result_critical, 
            bool  _result_guard, 
            float _result_hit_rate, 
            float _result_critical_rate,
            float _result_guard_rate, 
            int   _result_damage)
        {
            return new Combat_DamageResult
            {
                AttackerID          = _attacker_id,
                TargetID            = _target_id,
                WeaponID            = _weapon_id,
                Result_HP_Attacker  = _result_hp_attacker,
                Result_HP_Target    = _result_hp_target,
                WeaponEffectiveness = _weapon_effectiveness,
                Result_Hit          = _result_hit,
                Result_Critical     = _result_critical,
                Result_Guard        = _result_guard,
                Result_HitRate      = _result_hit_rate,
                Result_CriticalRate = _result_critical_rate,
                Result_GuardRate    = _result_guard_rate,
                Result_Damage       = _result_damage,
            };
        }
    }

    
    /// <summary>
    /// 전투 씬 관리용
    /// </summary>
    public class CombatSystemManager : Singleton<CombatSystemManager>, ISystemManager
    {
        Dictionary<int, CombatSystem> m_repository = new Dictionary<int, CombatSystem>();

        public ICombatSystemParam  Param      { get; private set; }
        public EnumState           State      { get; private set; }
        public bool                IsPause    { get; private set; } = false;


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;


        private bool                      m_is_logic_finished        = false;
        private bool                      m_is_post_process_finished = false;
        private List<Combat_DamageResult> m_list_damage_result       = new List<Combat_DamageResult>();


        protected override void Init()
        {
            base.Init();

            // 현재는 파엠을 의식하고 1대1 전투를 상정하고 만들어져 있음.


            // TODO: combatsyste_turn + combatsystem_damage 를 1개로 합쳐야 구조가 좀 깔끔할 것 같음.
            // 지금은 코드가 뭔가 좀 지저분하다....

            var turn_sytem         = new CombatSystem_Turn();        // 전투 공/방 순서 진행.
            var damage_sytem       = new CombatSystem_Damage();      // 전투 공/방 데미지 처리.
            var wand_sytem         = new CombatSystem_Wand();        // 전투 지팡이 처리.
            var post_process_sytem = new CombatSystem_PostProcess(); // 전투 연산 종료 후 연출 
            // var effect_sytem = new CombatSystem_Effect(); m_repository.Add((int)effect_sytem.SystemType, effect_sytem);

            m_repository.Add((int)turn_sytem.SystemType, turn_sytem);
            m_repository.Add((int)damage_sytem.SystemType, damage_sytem);
            m_repository.Add((int)wand_sytem.SystemType, wand_sytem);
            m_repository.Add((int)post_process_sytem.SystemType, post_process_sytem);

            foreach (var e in m_repository.Values)
                e.Init();
        }


        public void Release()
        {
            Param = null;
            State = EnumState.None;


            m_is_logic_finished = false;
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
            m_is_logic_finished = false;
            m_is_post_process_finished = false;
            m_list_damage_result.Clear();
            // Debug.LogWarning("list_damage_result.Clear()");


            // 공격자 / 방어자 무기 장착이 안되어 있을 경우, 자동 장착 처리.
            if (Param.CommandType == EnumUnitCommandType.Attack)
            {
                if (Param.Attacker != null)
                    Param.Attacker.Equip_Weapon_Auto();

                if (Param.Defender != null)
                    Param.Defender.Equip_Weapon_Auto();
            }
        }

        bool OnUpdate()
        {
            if (IsPause)
                return false;

            switch(Param.CommandType)
            {
                case EnumUnitCommandType.Attack:
                    OnUpdate_Combat_Logic();
                    break;
                case EnumUnitCommandType.Wand:
                    OnUpdate_Wand_Logic();
                    break;
            }
            
            OnUpdate_PostProcess();

            return m_is_logic_finished && m_is_post_process_finished;
        }


        void OnExit()
        {
            //Debug.Log("Battle Finished");

            // 모든 시스템이 순차적으로 완료처리된다는 보장이 없어서... 여기서 종료처리 합니다...;;;
            foreach (var e in m_repository.Values)
                e.Release();

            Param = null;
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
            if (m_is_logic_finished)
                return;


            // 공격자/방어자 턴 순서 계산.
            if (UpdateSystem(EnumSystem.CombatSystem_Turn, Param) == EnumState.Finished)
            {
                // 전투로직 종료 
                m_is_logic_finished = true;
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
                    m_is_logic_finished = true;
                    return;
                }
            }            
        }

        void OnUpdate_Wand_Logic()
        {
            if (m_is_logic_finished)
                return;

            if (UpdateSystem(EnumSystem.CombatSystem_Wand, Param) == EnumState.Finished)
            {
                m_is_logic_finished = true;
            }
        }

        void OnUpdate_PostProcess()
        {
            if (m_is_logic_finished == false)
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