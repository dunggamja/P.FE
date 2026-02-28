using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Battle
{
  public partial class CombatSystem_PostProcess : CombatSystem
  {
    private UniTask             m_task;
    private ManualUniTaskRunner m_task_runner = new ManualUniTaskRunner();


    public CombatSystem_PostProcess() : base(EnumSystem.CombatSystem_PostProcess)
    {

    }

    protected override void OnEnter(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
        //  Sequence 순서대로 연출 진행.
        m_task = PlaySequence(_param);
      }     
      else
      {
        m_task = UniTask.CompletedTask;
      } 
    }

    protected override bool OnUpdate(ICombatSystemParam _param)
    {
      //TODO: deltatime을 PARAM에 넣는게 좋을것 같음.
      m_task_runner.Tick(_param.DeltaTime);
      return m_task.Status != UniTaskStatus.Pending;
    }

    protected override void OnExit(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
        // 죽은 유닛 삭제 처리.
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


    async UniTask PlaySequence(ICombatSystemParam _param)
    {
      //  Battle Scene 시작.
      EventDispatchManager.Instance.UpdateEvent(
                  ObjectPool<Battle_Scene_ChangeEvent>
                  .Acquire()
                  .Set(true)
                  );   



      // 컷씬 이벤트 실행.
      CutsceneManager.Instance.OnPlayEvent(
          CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnCombatDirectionStart));

      // 컷씬 연출 체크.
      await UniTask.WaitUntil(()=> CutsceneManager.Instance.IsPlayingCutscene == false);


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

        await m_task_runner.DelayMS(500); // 연출 시간.
      }



      // 컷씬 이벤트 실행.
      CutsceneManager.Instance.OnPlayEvent(
          CutscenePlayEvent.Create(EnumCutscenePlayEvent.OnCombatDirectionEnd));

      // 컷씬 연출 체크.
      await UniTask.WaitUntil(()=> CutsceneManager.Instance.IsPlayingCutscene == false);

      //  Battle Scene 종료   .
      EventDispatchManager.Instance.UpdateEvent(
                    ObjectPool<Battle_Scene_ChangeEvent>.Acquire().Set(false)
                    );
    }
  }
}