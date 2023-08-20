using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Weapon : IWeapon
    {
        BaseContainer m_status    = new BaseContainer();
        BaseContainer m_attribute = new BaseContainer();

        public int  GetStatus(EnumWeaponStatus _status_type) => m_status.GetValue((int)_status_type);
        public bool HasAttribute(EnumWeaponAttribute _attribute_type) => m_attribute.HasValue((int)_attribute_type);
    }
}

