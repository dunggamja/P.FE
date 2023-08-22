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
        public bool    IsDead => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;

        public IBlackBoard          BlackBoard    { get; }
        public ISkill               Skill         { get; }
        public BattleStatusManager  StatusManager { get; }

        protected BattleObject(Int64 _id)
        {
            ID = _id;

            BlackBoard    = new BattleBlackBoard();
            Skill         = new BattleSkill();
            StatusManager = new BattleStatusManager(this);
        }

        public static BattleObject Create()
        {
            var    battle_object = new BattleObject(Util.GenerateID());

            return battle_object;
        }


        public void ApplyDamage(int _damage/*, bool _is_plan = false*/)
        {
            if (_damage <= 0)
                return;

            // TODO: 플랜에 대한 처리를 좀 더 깔끔하게 해보자...

            var cur_hp = StatusManager.Status.GetPoint(EnumUnitPoint.HP);
            var new_hp = Math.Max(0, cur_hp - _damage);

            StatusManager.Status.SetPoint(EnumUnitPoint.HP, new_hp);

            Debug.Log($"GetDamaged, ID:{ID}, HP:{new_hp}");
        }


    }

    public class BattleObjectManager : Singleton<BattleObjectManager>
    {
        Dictionary<Int64, BattleObject> m_repository_by_id = new Dictionary<long, BattleObject>();

        public BattleObject GetBattleObject(Int64 _id) 
        {
            if (m_repository_by_id.TryGetValue(_id, out var battle_object))
                return battle_object;

            return null;
        }

        public bool AddBattleObject(BattleObject _object)
        {
            if (_object == null)
                return false;

            if (m_repository_by_id.ContainsKey(_object.ID))
                return false;

            m_repository_by_id.Add(_object.ID, _object);
            return true;
        }
    }
}

