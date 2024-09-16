using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SensorManager
    {
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
    }
}