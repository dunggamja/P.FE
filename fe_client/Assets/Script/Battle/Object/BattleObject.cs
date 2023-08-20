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

        public void ApplyDamage(int _damage)
        {
            if (_damage <= 0)
                return;

            // 0 미만으로는 내리지 않는다.
            HP = Math.Max(0, HP - _damage);
        }

        public void ApplyHeal(int _heal)
        {
            if (_heal <= 0)
                return;

            // HPMax 이상은 제한.
            HP = Math.Min(HP + _heal, HPMax);
        }
    }

    public class BattleObjectManager : Singleton<BattleObjectManager>
    {
        Dictionary<Int64, BattleObject> m_repository_by_id = new Dictionary<long, BattleObject>();

        public BattleObject GetByID(Int64 _id) => m_repository_by_id.TryGetValue(_id, out var battle_object) ? battle_object : null;
    }
}

