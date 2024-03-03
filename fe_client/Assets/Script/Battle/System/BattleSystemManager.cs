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


        public IBattleSystemParam  Param      { get; private set; }
        public EnumState           State      { get; private set; }


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;


        protected override void Init()
        {
            base.Init();

    

            // 실행할 순서대로 넣어놓는다.
            m_update_system.Add(new BattleSystem_Turn());       
            m_update_system.Add(new BattleSystem_Command());    


            m_update_system.ForEach(e => e.Init());
            
        }

        public void Release()
        {
            Param = null;
            State = EnumState.None;

            m_update_system.ForEach(e => e.Release());
            //m_update_index = 0;
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
            m_update_system.ForEach(e => e.Update(Param));            

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


    }
}
