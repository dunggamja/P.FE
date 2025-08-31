using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class AIManager
    {
        // TODO: sensor system에 대해서 정리 필요함...;;;;;
        // 현재는 그냥 공용함수로 돌리는 거랑 차이가 없는 상태임....

        public bool Initialize(IOwner _owner)
        {
            AddAIUpdater(new AI_Attack());
            AddAIUpdater(new AI_Done());

            return true;
        }


        private List<(System.Type _type, IAIUpdater _ai)> m_repository = new(10);

        public bool AddAIUpdater(IAIUpdater _ai)
        {
            if (_ai == null)
                return false;

            var type        = _ai.GetType();
            var find_index  = m_repository.FindIndex((e) => { return e._type == type; });
            if (find_index >= 0)
                return false;


            m_repository.Add((type, _ai));
            return true;
        }

        public T GetAIUpdater<T>() where T : class, IAIUpdater 
        {
            var find_index  = m_repository.FindIndex((e) => { return e._type == typeof(T);});
            if (find_index >= 0)
                return m_repository[find_index]._ai as T;

            return null;
        }

        public bool RemoveAIUpdater<T>() where T : class, IAIUpdater 
        {
            var find_index = m_repository.FindIndex((e) => { return e._type == typeof(T);});
            if (find_index < 0)
                return false;

            m_repository.RemoveAt(find_index);
            return true;
        }

        public T TryAddAIUpdater<T>() where T : class, IAIUpdater, new()
        {
            var ai = GetAIUpdater<T>();
            if (ai == null)
            {
                if(!AddAIUpdater(new T()))
                    return null;

                ai = GetAIUpdater<T>();
            }

            return ai;            
        }


        public void Update(IOwner _owner)
        {
            foreach((var type, var ai) in m_repository)
            {
                ai.Update(_owner);
            }
        }
    }
}