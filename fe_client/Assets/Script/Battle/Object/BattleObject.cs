using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleObject : IOwner
    {
        public Int64   ID     { get; private set; }
        public ITarget Target { get; }
        public bool    IsDead { get; }

        public IBlackBoard  BlackBoard { get; }
        public BattleStatus Status     { get; }
        public BattleSkill  Skill      { get; }

        public int HP    { get; private set; }
        public int HPMax { get; private set; }

        public void AddHP(int _value)
        {
            // 0 ~ MaxHP 사이로 제한합니다.
            HP = Mathf.Clamp(HP + _value, 0, HPMax);
        }
    }

    public class BattleObjectManager : Singleton<BattleObjectManager>
    {
        Dictionary<Int64, BattleObject> m_repository_by_id = new Dictionary<long, BattleObject>();

        public BattleObject GetByID(Int64 _id) => m_repository_by_id.TryGetValue(_id, out var battle_object) ? battle_object : null;
    }
}

