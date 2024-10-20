using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SensorManager
    {
        // TODO: sensor system에 대해서 정리 필요함...;;;;;
        // 현재는 그냥 공용함수로 돌리는 거랑 차이가 없는 상태임....

        Int64 m_owner_id = 0;


        public bool Initialize(IOwner _owner)
        {
            if (_owner == null)
                return false;

            m_owner_id = _owner.ID;

            AddSensor(new Sensor_Target_Score());

            return true;
        }


        private List<(System.Type _type, ISensor _sensor)> m_repository = new(10);

        public bool AddSensor(ISensor _sensor)
        {
            if (_sensor == null)
                return false;

            var type        = _sensor.GetType();
            var find_index  = m_repository.FindIndex((e) => { return e._type == type; });
            if (find_index >= 0)
                return false;


            m_repository.Add((type, _sensor));
            return true;
        }

        public T GetSensor<T>() where T : class, ISensor 
        {
            var find_index  = m_repository.FindIndex((e) => { return e._type == typeof(T);});
            if (find_index >= 0)
                return m_repository[find_index]._sensor as T;

            return null;
        }

        public bool RemoveSensor<T>() where T : class, ISensor 
        {
            var find_index = m_repository.FindIndex((e) => { return e._type == typeof(T);});
            if (find_index < 0)
                return false;

            m_repository.RemoveAt(find_index);
            return true;
        }

        public T TryAddSensor<T>() where T : class, ISensor, new()
        {
            var sensor = GetSensor<T>();
            if (sensor == null)
            {
                if(!AddSensor(new T()))
                    return null;

                sensor = GetSensor<T>();
            }

            return sensor;            
        }


        public void Update()
        {
            var entity = EntityManager.Instance.GetEntity(m_owner_id);
            if (entity == null)
                return;

            foreach((var type, var sensor) in m_repository)
            {
                sensor.Update(entity);
            }
        }
    }
}