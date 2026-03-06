using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    // 전투 연출을 위한 정보들.

    // 결과 정보 (HP, 명중여부, 필살여부, 방어여부, 데미지)
    public struct CombatRecord_Result
    {
        public int    HP_Attacker { get; private set; }
        public int    HP_Target   { get; private set; }   
        public bool   Hit         { get; private set; }
        public bool   Critical    { get; private set; }
        public bool   Guard       { get; private set; }
        public int    Damage      { get; private set; }   

        public static CombatRecord_Result Create(
            int _hp_attacker,
            int _hp_target,
            bool _hit,
            bool _critical,
            bool _guard,
            int _damage)
        {
            return new CombatRecord_Result
            {
                HP_Attacker = _hp_attacker,
                HP_Target   = _hp_target,
                Hit         = _hit,
                Critical    = _critical,
                Guard       = _guard,
                Damage      = _damage,
            };
        }
    }

    // 확률 정보 (명중, 필살, 방어)
    public struct CombatRecord_Rate
    {
        public float  HitRate             { get; private set; }
        public float  CriticalRate        { get; private set; }
        public float  GuardRate           { get; private set; }        
        public bool   WeaponEffectiveness { get; private set; }

        public int    HitRate_Percent      => Math.Clamp((int)(HitRate * 100), 0, 100);
        public int    CriticalRate_Percent => Math.Clamp((int)(CriticalRate * 100), 0, 100);
        public int    GuardRate_Percent    => Math.Clamp((int)(GuardRate * 100), 0, 100);

        public static CombatRecord_Rate Create(
            float _hit_rate,
            float _critical_rate,
            float _guard_rate,
            bool _weapon_effectiveness)
        {
            return new CombatRecord_Rate
            {
                HitRate             = _hit_rate,
                CriticalRate        = _critical_rate,
                GuardRate           = _guard_rate,
                WeaponEffectiveness = _weapon_effectiveness,
            };
        }
    }

    // 아이템 변도사항 (강탈/파괴 등)
    public struct CombatRecord_Item
    {
        public enum EnumInventoryActionType
        {
            None, Steal, Destroy, Disarm, Acquire
        }

        public EnumInventoryActionType ActionType { get; private set; }
        public Int64 EntityID  { get; private set; }
        public int   ItemKind  { get; private set; }
        public int   ItemValue { get; private set; }

        public static CombatRecord_Item Create(
            EnumInventoryActionType _action_type,
            Int64                   _entity_id,
            int                     _item_kind,
            int                     _item_value)
        {
            return new CombatRecord_Item
            {
                ActionType = _action_type,
                EntityID   = _entity_id,
                ItemKind   = _item_kind,
                ItemValue  = _item_value,
            };
        }
    }


    public struct CombatRecord
    {
        // 공격자 / 타겟 / 무기 ID
        public Int64  AttackerID         { get; private set; }
        public Int64  TargetID           { get; private set; }
        public Int64  WeaponID           { get; private set; }        

        public CombatRecord_Result Result { get; private set; }

        public CombatRecord_Rate   Rate { get; private set; }

        public CombatRecord_Item   Item { get; private set; }

        public static CombatRecord Create(
            Int64 _attacker_id,
            Int64 _target_id,
            Int64 _weapon_id)
        {
            return new CombatRecord
            {
                AttackerID          = _attacker_id,
                TargetID            = _target_id,
                WeaponID            = _weapon_id
            };
        }

        public void SetResult(CombatRecord_Result _result)
        {
            Result = _result;
        }

        public void SetRate(CombatRecord_Rate _rate)
        {
            Rate = _rate;
        }

        public void SetItem(CombatRecord_Item _item)
        {
            Item = _item;
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
        public bool                IsPause    => false;


        public    float            DeltaTime { get; private set; } = 0f;
        private   float            m_last_time = 0f;


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;


        private bool                      m_is_logic_finished        = false;
        private bool                      m_is_post_process_finished = false;
        private List<CombatRecord> m_list_record       = new List<CombatRecord>();


        protected override void Init()
        {
            base.Init();

            // 현재는 파엠을 의식하고 1대1 전투를 상정하고 만들어져 있음.

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
            m_list_record.Clear();


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
            m_list_record.Clear();
            // Debug.LogWarning("list_damage_result.Clear()");


            // 공격자 / 방어자 무기 장착이 안되어 있을 경우, 자동 장착 처리.
            // TODO: Berwick에서는 안해도 될듯.
            if (Param.CommandType == EnumUnitCommandType.Attack)
            {
                if (Param.Attacker != null)
                    Param.Attacker.Equip_Weapon_Auto();

                if (Param.Defender != null)
                    Param.Defender.Equip_Weapon_Auto();
            }


            // 컷씬 이벤트 실행.
            if (Param.IsPlan == false)
            {                
                CutsceneManager.Instance.OnPlayEvent(
                    CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnCombatStart));
            }            
        }

        bool OnUpdate()
        {
            if (m_last_time > 0f)
            {
                DeltaTime = Time.time - m_last_time;
            }

            m_last_time = Time.time;


            if (IsPause)
                return false;

            switch(Param.CommandType)
            {
                // 전투 로직.
                case EnumUnitCommandType.Attack:
                    OnUpdate_Combat_Logic();
                    break;

                // 지팡이 로직.
                case EnumUnitCommandType.Wand:
                    OnUpdate_Wand_Logic();
                    break;
            }
            
            // 전투 종료 후 처리.
            OnUpdate_PostProcess();

            return m_is_logic_finished && m_is_post_process_finished;
        }


        void OnExit()
        {
            //Debug.Log("Battle Finished");

            if (Param.IsPlan == false)
            {
                // 컷씬 이벤트 실행.
                CutsceneManager.Instance.OnPlayEvent(
                    CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnCombatEnd));
            }  

            // 모든 시스템이 순차적으로 완료처리된다는 보장이 없어서... 여기서 종료처리 합니다...;;;
            foreach (var e in m_repository.Values)
                e.Release();

            Param       = null;
            m_last_time =  0f;
            DeltaTime   =  0f;
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


        public void AddCombatRecord(CombatRecord _record)
        {
            m_list_record.Add(_record);
        }

        public List<CombatRecord> GetCombatRecord()
        {
            return m_list_record;
        }



        public bool IsEngaged(Int64 _id)  => IsAttacker(_id) || IsDefender(_id);
        public bool IsAttacker(Int64 _id) => (Param != null && Param.Attacker != null && Param.Attacker.ID == _id) && 0 < _id;
        public bool IsDefender(Int64 _id) => (Param != null && Param.Defender != null && Param.Defender.ID == _id) && 0 < _id;

        public Int64 AttackerID => Param?.Attacker?.ID ?? 0;
        public Int64 DefenderID => Param?.Defender?.ID ?? 0;



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