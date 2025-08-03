using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public struct BattleLog
    {
        public enum EnumLogType
        {
            None = 0,

            Battle_Turn_Start = 1,
            Battle_Turn_End,


            Combat_Turn_Start = 1000,
            Combat_Turn_End,

            Combat_Attack,
            Combat_Heal,
            Combat_Guard,
            Combat_Evade,
        }

        public EnumLogType LogType  { get; private set; }
        public Int64       ActorID  { get; private set; }
        public Int64       TargetID { get; private set; }
        public int         Value    { get; private set; }


        public static BattleLog Create(EnumLogType _log_type, Int64 _actor_id, Int64 _target_id, int _value)
        {
            return new BattleLog 
            { 
                LogType  = _log_type, 
                ActorID  = _actor_id, 
                TargetID = _target_id, 
                Value    = _value 
            };
        }

    }

    public class BattleLogManager : Singleton<BattleLogManager>
    {
        private List<BattleLog> m_logs = new();

        public void AddLog(BattleLog _log)
        {
            m_logs.Add(_log);
        }

        public void Clear()
        {
            m_logs.Clear();
        }

        public BattleLogManager_IO Save()
        {            
            return new BattleLogManager_IO { Logs = new List<BattleLog>(m_logs) };
        }

        public void Load(BattleLogManager_IO _io)
        {
            if (_io == null)
                return;

            m_logs.Clear();
            m_logs.AddRange(_io.Logs);
        }
    }


    public class BattleLogManager_IO
    {
      public List<BattleLog> Logs { get; set; }
    }




}