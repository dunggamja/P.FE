using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Battle
{
  public partial class CombatSystem_PostProcess : CombatSystem
  {
    private bool m_sequence_is_done = false;
    


    public CombatSystem_PostProcess() : base(EnumSystem.CombatSystem_PostProcess)
    {

    }

    protected override void OnEnter(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
        m_sequence_is_done = false;
        
         //  Sequence 순서대로 연출 진행.
         PlaySequence().Forget();
      }     
      else
      {
        // 연출 할 것이 없음.
        m_sequence_is_done = true;
      } 
    }

    protected override bool OnUpdate(ICombatSystemParam _param)
    {
      
      return m_sequence_is_done;
    }

    protected override void OnExit(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
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


    async UniTask PlaySequence()
    {
      //  Battle Scene 시작.
      EventDispatchManager.Instance.UpdateEvent(
                  ObjectPool<Battle_Scene_ChangeEvent>
                  .Acquire()
                  .Set(true)
                  );        


      var list_damage_result = CombatSystemManager.Instance.GetCombatDamageResult();

      for (int i = 0; i < list_damage_result.Count; i++)
      {
        var damage    = list_damage_result[i];

        var target_id = damage.TargetID;
        var target_hp = damage.Result_HP_Target;

        // 타겟 HP 갱신.
        EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Entity_HP_UpdateEvent>
                .Acquire()
                .Set(target_id, target_hp)
                );

        await UniTask.Delay(500); // 연출 시간.
      }


      //  Battle Scene 종료   .
      EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Scene_ChangeEvent>.Acquire().Set(false)
                    );

      // 연출 완료.
      m_sequence_is_done = true;
    }
  }
}