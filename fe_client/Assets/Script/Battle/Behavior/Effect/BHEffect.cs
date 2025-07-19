using System;
using System.Collections;
using System.Collections.Generic;
using TacticsToolkit;
using UnityEngine;


namespace Battle
{
    public enum EnumBHEffectType
    {
        None,
        Add,
        Subtract,
        Set,
        True,
        False,
        
    }


    public abstract class BHEffect
    {
        public abstract void Apply(IBHParam _param);
    }


    public class BHEffect_Status : BHEffect
    {
        public EnumBHEffectType EffectType { get; private set;  } = EnumBHEffectType.None;
        public EnumUnitStatus   StatusType { get; private set;  } = EnumUnitStatus.None;
        public int              Value      { get; private set;  } = 0;

        public BHEffect_Status(EnumBHEffectType _effect_type, EnumUnitStatus _status_type, int _value)
            : base()
        {
            EffectType = _effect_type;
            StatusType = _status_type;
            Value      = _value;
        }

        public override void Apply(IBHParam _param)
        {
            var target_id = _param?.Target?.MainTargetID ?? 0;
            if (target_id == 0)
                return;

            var target = EntityManager.Instance.GetEntity(target_id);
            if (target == null)
                return;

            var prev_value = target.StatusManager.Status.GetStatus(StatusType, _param.IsPlan);
            var new_value  = 0;

            switch (EffectType)
            {
                case EnumBHEffectType.Add:      new_value = prev_value + Value; break;
                case EnumBHEffectType.Subtract: new_value = prev_value - Value; break;
                case EnumBHEffectType.Set:      new_value = Value;              break;
                case EnumBHEffectType.True:     new_value = 1;                  break;
                case EnumBHEffectType.False:    new_value = 0;                  break;
            }

            target.StatusManager.Status.SetStatus(StatusType, new_value, _param.IsPlan);
        }

    }
}