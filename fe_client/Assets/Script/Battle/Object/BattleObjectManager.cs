using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
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


        public void Loop(Action<BattleObject> _callback)
        {
            if (_callback == null)
                return;

            foreach(var e in m_repository_by_id.Values)
            {
                _callback(e);
            }
        }

        public BattleObject Find(Func<BattleObject, bool> _callback)
        {
            if (_callback == null)
                return null;

            foreach (var e in m_repository_by_id.Values)
            {
                if (_callback(e))
                    return e;
            }

            return null;
        }

        public void Update()
        {
            foreach(var e in m_repository_by_id.Values)
            {
                e.Update(Time.deltaTime);
            }
        }

    }
}
