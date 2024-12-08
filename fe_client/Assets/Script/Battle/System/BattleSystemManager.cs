using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleSystemManager : Singleton<BattleSystemManager>, ISystemManager
    {
        // Dictionary<int, BattleSystem> m_repository = new Dictionary<int, BattleSystem>();

        List<BattleSystem>      m_update_system = new();


        public IBattleSystemParam                Param               { get; private set; }
        public EnumState                         State               { get; private set; }
                       
        public BattleBlackBoard                  BlackBoard          { get; private set; } = new ();

        


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;

        private int                                m_system_index      = 0;
        private Dictionary<int, EnumCommanderType> m_faction_commander = new ();


        protected override void Init()
        {
            base.Init();

    

            // 실행할 순서대로 넣어놓는다.

            // 턴 진행
            m_update_system.Add(new BattleSystem_Turn());       
            // 명령 생성
            m_update_system.Add(new BattleSystem_Command_Dispatch());    
            // 명령 처리
            m_update_system.Add(new BattleSystem_Command_Progress());



            m_update_system.ForEach(e => e.Init());
            
        }

        public void Release()
        {
            Param = null;
            State = EnumState.None;
            BlackBoard.Reset();

            m_update_system.ForEach(e => e.Release());
            m_system_index = 0;

            m_faction_commander.Clear();
        }

        public void SetData(IBattleSystemParam _param)
        {
            Param = _param;
        }


        void OnEnter()
        {
            m_system_index = 0;
        }

        bool OnUpdate()
        {
            // 시스템을 순차적으로 실행해야 한다.
            for(; m_system_index < m_update_system.Count; ++m_system_index)
            {
                // 진해중인 시스템이 있다면 다음 프레임에 이어서..
                if (m_update_system[m_system_index].Update(Param) == EnumState.Progress)
                    break;
           } 

            // 다시 처음부터.
            if (m_system_index >= m_update_system.Count)
            {
                m_system_index  = 0;
            }


            // 종료 타이밍은 변수를 따로 둬야할듯.
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
            foreach(var e in m_update_system)
            {
                if (e.SystemType == _system_type)
                    return e;
            }

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


        public EnumCommanderType GetFactionCommanderType(int _faction)
        {
            if (m_faction_commander.TryGetValue(_faction, out var command_owner))
                return command_owner;

            return EnumCommanderType.None;
        }

        public void SetFactionCommanderType(int _faction, EnumCommanderType _commander_type)
        {
            if (!m_faction_commander.ContainsKey(_faction))
                 m_faction_commander.Add(_faction, _commander_type);
            else
                 m_faction_commander[_faction] = _commander_type;
        }


    }
}
