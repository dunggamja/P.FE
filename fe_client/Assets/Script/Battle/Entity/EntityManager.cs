using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public List<Entity> Collect(Func<Entity, bool> _callback)
        {
            var list_result = new List<Entity>(10);
            if (_callback != null)
            {
                foreach(var e in m_repository_by_id.Values)
                {
                    if (_callback(e))
                        list_result.Add(e);
                }
            }

            return list_result;
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

        // public void Update()
        // {
        //     foreach(var e in m_repository_by_id.Values)
        //     {
        //         e.Update(Time.deltaTime);
        //     }
        // }

        void Clear()
        {
            m_repository_by_id.Clear();
        }



        public EntityManager_IO Save()
        {
            var snapshot = new EntityManager_IO();

            foreach(var e in m_repository_by_id.Values)
            {
                snapshot.Entities.Add(e.Save());
                snapshot.ActiveID.Add(e.ID);
            }

            return snapshot;
        }

        public void Load(EntityManager_IO _snapshot)
        {
            if (_snapshot == null)
                return;



            foreach(var e in _snapshot.Entities)
            {
                var entity = GetEntity(e.ID);
                if (entity == null)
                {
                    entity = Entity.Create(e.ID);
                    AddEntity(entity);
                }

                if (entity != null)
                    entity.Load(e);    
            }

            // 스냅샷에 포함이 안 된 항목들은 제거해줍시다.
            {
                var list_delete =  ListPool<Int64>.Acquire();
                foreach((var id, var e) in m_repository_by_id)
                {
                    if (!_snapshot.ActiveID.Contains(id))
                        list_delete.Add(id);
                }
                // 삭제.~
                foreach(var id in list_delete)
                {
                    Remove(id);

                    // 오브젝트도 삭제 처리.
                    WorldObjectManager.Instance.DeleteObject(id);
                }

                ListPool<Int64>.Return(list_delete);
            }

            // 오브젝트가 없는 친구들은 만들어 줍시다.
            foreach(var e in m_repository_by_id.Values)
            {
                var world_object = WorldObjectManager.Instance.Seek(e.ID);
                if (world_object == null)
                    WorldObjectManager.Instance.CreateObject(e.ID).Forget();
            }

        }

    }


    public class EntityManager_IO
    {


        public List<Entity_IO> Entities { get; private set; } = new();    


        // 현재 생존해 있는 ID 목록
        public HashSet<Int64>  ActiveID { get; private set; } = new();
    }
}
