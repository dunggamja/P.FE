using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
  public partial class CombatSystem_PostProcess : CombatSystem
  {
    public CombatSystem_PostProcess() : base(EnumSystem.CombatSystem_PostProcess)
    {

    }

    protected override void OnEnter(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
        // 불필요한 GUI들 닫기.
        EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Scene_ChangeEvent>
                    .Acquire()
                    .Set(true)
                    );
      }      
    }

    protected override bool OnUpdate(ICombatSystemParam _param)
    {
      return true;
    }

    protected override void OnExit(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
        EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Scene_ChangeEvent>
                    .Acquire()
                    .Set(false)
                    );


        if (_param.Attacker != null && _param.Attacker.IsDead)
            _param.Attacker.DeleteProcess();

        if (_param.Defender != null && _param.Defender.IsDead)
            _param.Defender.DeleteProcess();
        
      }
    }
    
    protected override void OnInit()
    {
    }

    protected override void OnRelease()
    {
    }


  }
}