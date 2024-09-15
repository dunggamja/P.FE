using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class EntityManager : Singleton<EntityManager>
    {
        Dictionary<Int64, Entity> m_repository_by_id = new Dictionary<long, Entity>();

        public Entity CreateEntity(Int64 _id)
        {
            if (_id == 0)
                _id = Util.GenerateID();

            var new_entity = Entity.Create(_id);
            if (!AddEntity(new_entity))
                return null;
            
            return new_entity;
        }

        public Entity GetEntity(Int64 _id)
        {
            if (m_repository_by_id.TryGetValue(_id, out var battle_object))
                return battle_object;

            return null;
        }

        public bool AddEntity(Entity _object)
        {
            if (_object == null)
                return false;

            if (m_repository_by_id.ContainsKey(_object.ID))
                return false;

            m_repository_by_id.Add(_object.ID, _object);
            return true;
        }

        public bool Remove(Int64 _id)
        {
            return m_repository_by_id.Remove(_id);
        }


        public void Loop(Action<Entity> _callback)
        {
            if (_callback == null)
                return;

            foreach(var e in m_repository_by_id.Values)
            {
                _callback(e);
            }
        }

        public Entity Find(Func<Entity, bool> _callback)
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
