using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleObject : IOwner
    {
        public Int64             ID { get; private set; }

        public IBlackBoard  BlackBoard { get; }
        public BattleStatus Status     { get; }
        public BattleSkill  Skill      { get; }

        //public bool IsDead { get; }
    }

    public class BattleObjectManager : Singleton<BattleObjectManager>
    {
        Dictionary<Int64, BattleObject> m_repository_by_id = new Dictionary<long, BattleObject>();

        public BattleObject GetByID(Int64 _id) => m_repository_by_id.TryGetValue(_id, out var battle_object) ? battle_object : null;
    }
}

