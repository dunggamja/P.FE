using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class UnitStatus : IStatus
    {
        BaseContainer m_point_repository     = new BaseContainer();
        BaseContainer m_status_repository    = new BaseContainer();
        BaseContainer m_attribute_repository = new BaseContainer();

        public int GetPoint(EnumUnitPoint _point_type)
        {
            return m_point_repository.GetValue((int)_point_type);
        }

        public int GetStatus(EnumUnitStatus _status_type)
        {
            return m_status_repository.GetValue((int)_status_type);
        }

        public bool HasAttribute(EnumUnitAttribute _attribute_type)
        {
            return m_attribute_repository.HasValue((int)_attribute_type);
        }

        public void SetPoint(EnumUnitPoint _point_type, int _value)
        {
            m_point_repository.SetValue((int)_point_type, _value);
        }

        public void SetStatus(EnumUnitStatus _status_type, int _value)
        {
            m_status_repository.SetValue((int)_status_type, _value);
        }

        public void SetAttribute(EnumUnitAttribute _attribute_type, bool _value)
        {
            m_attribute_repository.SetValue((int)_attribute_type, _value);
        }
    }
}