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

        public BaseContainer Points { get; }


        public void ApplyDamage(int _damage/*, bool _is_plan = false*/)
        {
            if (_damage <= 0)
                return;

            var cur_hp = Points.GetValue((int)EnumUnitPoint.HP/*, _is_plan*/);
            var new_hp = Math.Max(0, cur_hp - _damage);

            //if (_is_plan)
            //    Points.PushPlanValue((int)EnumUnitPoint.HP, new_hp);
            //else
            
            // TODO: 플랜에 대한 처리를 좀 더 깔끔하게 해야하는데....

            Points.SetValue((int)EnumUnitPoint.HP, new_hp);
        }
        
    }

    public class BattleObjectManager : Singleton<BattleObjectManager>
    {
        Dictionary<Int64, BattleObject> m_repository_by_id = new Dictionary<long, BattleObject>();

        public BattleObject GetByID(Int64 _id) => m_repository_by_id.TryGetValue(_id, out var battle_object) ? battle_object : null;
    }
}

