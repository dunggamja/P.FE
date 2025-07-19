using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class UnitStatus //: IStatus
    {
        BaseContainer m_point_repository     = new BaseContainer();
        BaseContainer m_status_repository    = new BaseContainer();
        BaseContainer m_attribute_repository = new BaseContainer();

        public int GetPoint(EnumUnitPoint _point_type, bool _is_plan = false)
        {
            return m_point_repository.GetValue((int)_point_type, _is_plan);
        }

        public int GetStatus(EnumUnitStatus _status_type, bool _is_plan = false)
        {
            return m_status_repository.GetValue((int)_status_type, _is_plan);
        }

        public bool HasAttribute(EnumUnitAttribute _attribute_type, bool _is_plan = false)
        {
            return m_attribute_repository.HasValue((int)_attribute_type, _is_plan);
        }

        public void SetPoint(EnumUnitPoint _point_type, int _value, bool _is_plan = false)
        {
            m_point_repository.SetValue((int)_point_type, _value, _is_plan);
        }

        public void SetStatus(EnumUnitStatus _status_type, int _value, bool _is_plan = false)
        {
            m_status_repository.SetValue((int)_status_type, _value, _is_plan);
        }

        public void SetAttribute(EnumUnitAttribute _attribute_type, bool _value, bool _is_plan = false)
        {
            m_attribute_repository.SetValue((int)_attribute_type, _value, _is_plan);
        }
    }
}