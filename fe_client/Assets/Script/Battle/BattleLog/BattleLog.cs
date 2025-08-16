using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumBattleLogType
    {
        None = 0,

        Battle_Turn_Start = 1,
        Battle_Turn_End,


        Combat_Turn_Start,
        Combat_Turn_End,

        Damage,
        Heal,
        Guard,
        Evade,
    }

    public struct BattleLog
    {
        public EnumBattleLogType LogType  { get; private set; }
        public Int64             EntityID { get; private set; }
        public int               Value    { get; private set; }


        public static BattleLog Create(EnumBattleLogType _log_type, Int64 _entity_id, int _value)
        {
            return new BattleLog 
            { 
                LogType  = _log_type, 
                EntityID = _entity_id, 
                Value    = _value 
            };
        }

    }

    public class BattleLogManager : Singleton<BattleLogManager>
    {
        public List<BattleLog> Logs { get; private set; } = new();

        public void AddLog(EnumBattleLogType _log_type, Int64 _entity_id, int _value)
        {
            Logs.Add(BattleLog.Create(_log_type, _entity_id, _value));
        }

        public void AddLog(BattleLog _log)
        {
            Logs.Add(_log);
        }

        public void Clear()
        {
            Logs.Clear();
        }

        public void CopyLogs(ref List<BattleLog> _logs)
        {
            _logs.Clear();
            _logs.AddRange(Logs);
        }

        public BattleLogManager_IO Save()
        {            
            return new BattleLogManager_IO { Logs = new List<BattleLog>(Logs) };
        }

        public void Load(BattleLogManager_IO _io)
        {
            if (_io == null)
                return;

            Logs.Clear();
            Logs.AddRange(_io.Logs);
        }
    }


    public class BattleLogManager_IO
    {
      public List<BattleLog> Logs { get; set; }
    }




}