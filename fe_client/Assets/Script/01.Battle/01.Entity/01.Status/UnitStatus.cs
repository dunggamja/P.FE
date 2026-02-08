using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class UnitStatus //: IStatus
    {
        public int    ClassKIND { get ; private set; } = 0;
        
        BaseContainer m_point_repository     = new BaseContainer();
        BaseContainer m_status_repository    = new BaseContainer();

        public void SetClassKIND(int _class_kind)
        {
            ClassKIND = _class_kind;
        }

        public int GetPoint(EnumUnitPoint _point_type)
        {
            return (int)m_point_repository.GetValue((int)_point_type);
        }

        public int GetStatus(EnumUnitStatus _status_type)
        {
            return (int)m_status_repository.GetValue((int)_status_type);
        }

        public bool HasAttribute(EnumUnitAttribute _attribute_type)
        {
            return DataManager.Instance.UnitSheet.HasClassAttribute_Unit(ClassKIND, _attribute_type);
        }

        public void SetPoint(EnumUnitPoint _point_type, int _value)
        {
            m_point_repository.SetValue((int)_point_type, _value);
        }

        public void SetStatus(EnumUnitStatus _status_type, int _value)
        {
            m_status_repository.SetValue((int)_status_type, _value);
        }

        // public int GetPathAttribute()
        // {
        //     return DataManager.Instance.UnitSheet.GetClassAttribute_Path(ClassKIND);
        // }

        public int GetTerrainCost(EnumTerrainAttribute _terrain_attribute)
        {
            return 0;
        }

        public bool IsvalidStopTerrain(EnumTerrainAttribute _terrain_attribute)
        {
            // HasAttribute()


            //*1 Cannot end movement in that terrain
            return false;
        }

        public bool HasClassAttribute_Weapon(EnumWeaponCategory _weapon_category)
        {
            return DataManager.Instance.UnitSheet.HasClassAttribute_Weapon(ClassKIND, _weapon_category);
        }

        public UnitStatus_IO Save()
        {
            return new UnitStatus_IO()
            {
                ClassKIND = ClassKIND,
                Point     = m_point_repository.Save(),
                Status    = m_status_repository.Save() 
            };
        }

        public void Load(UnitStatus_IO _snapshot)
        {
            ClassKIND = _snapshot.ClassKIND;
            m_point_repository.Load(_snapshot.Point);
            m_status_repository.Load(_snapshot.Status);
            // m_attribute_repository.Load(_snapshot.Attribute);
        }
    }

    public class UnitStatus_IO
    {
        public int              ClassKIND { get; set; } = 0;
        public BaseContainer_IO Point     { get; set; } = new();
        public BaseContainer_IO Status    { get; set; } = new();
        // public BaseContainer_IO Attribute { get; set; } = new();
    }
}