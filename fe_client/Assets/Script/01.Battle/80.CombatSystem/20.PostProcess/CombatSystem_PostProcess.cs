using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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
      m_task_runner.Tick(_param.DeltaTime);
      return m_task.Status != UniTaskStatus.Pending;
    }

    protected override void OnExit(ICombatSystemParam _param)
    {
      if (_param.IsPlan == false)
      {
        // TODO: 강탈 등 아이템 처리.


        // 죽은 유닛 삭제 처리.
        if (_param.Attacker != null && _param.Attacker.IsDead)
            _param.Attacker.DeleteProcess();

        // 죽은 유닛 삭제 처리.
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


      var list_damage_result = CombatSystemManager.Instance.GetCombatRecord();

      for (int i = 0; i < list_damage_result.Count; i++)
      {
        var damage    = list_damage_result[i];

        var target_id = damage.TargetID;
        var target_hp = damage.Result.HP_Target;

        // 타겟 HP 갱신.
        EventDispatchManager.Instance.UpdateEvent(
                ObjectPool<Battle_Entity_HP_UpdateEvent>
                .Acquire()
                .Set(target_id, target_hp)
                );

        await m_task_runner.DelayMS(500); // 연출 시간... 현재는 하드코딩...
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

      // 드랍 아이템 획득 처리.      
      await PlaySequence_DropItem(_param);
    }
  
  
    async UniTask PlaySequence_DropItem(ICombatSystemParam _param)
    {
        using var list_drop_item = ListPool<Item>.AcquireWrapper();           

        // 죽은 유닛의 인벤토리 아이템중 드랍 아이템들을 모아봅시다.
        // TODO: 포획일경우, 모든 아이템을 획득하도록 합시다.
        if (_param.Attacker != null && _param.Attacker.IsDead)
            _param.Attacker.Inventory.CollectItem(list_drop_item.Value, e => e.IsDrop);

        if (_param.Defender != null && _param.Defender.IsDead)
            _param.Defender.Inventory.CollectItem(list_drop_item.Value, e => e.IsDrop);

        // 드랍 아이템이 있다면 아이템 획득을 처리합니다.
        if (list_drop_item.Value.Count == 0)
          return;

        // 생존한 유닛에게 아이템을 줍시다.
        var alive_unit = (_param.Attacker != null && _param.Attacker.IsDead == false) ? _param.Attacker : _param.Defender;    
        if (alive_unit == null)
          return;

        // 아이템 획득 처리.
        foreach(var item in list_drop_item.Value)
          ItemHelper.AcquireItem(alive_unit, item);


        // TODO: 나중에 다른 UI로 교체하자. 현재는 대화 UI로 연출 처리.
        await DialoguePublisher.TryOpenUI(CancellationToken.None);

        // 아이템 획득 팝업.
        for(int i = 0; i < list_drop_item.Value.Count; i++)
        {
          // 대화 데이터 생성.
          DIALOGUE_SEQUENCE dialogue_data = new();
          dialogue_data.SetID(Util.GenerateID());
          
          var item = (i < list_drop_item.Value.Count) ? list_drop_item.Value[i] : null;
          if (item != null)
          {
            // 아이템 획득 메시지.
            var item_name      = item.GetLocalizeName();
            var item_name_text = await LocalizationManager.Instance.GetTextAsync(item_name.Table, item_name.Key);            
            dialogue_data.SetCloseDialogue(false);
            dialogue_data.AddDialogueData(new List<DIALOGUE_DATA>()
            {
              new DIALOGUE_DATA()
              {
                IsActive = true,
                Position = DIALOGUE_DATA.EnumPosition.Center,
                Dialogue = item_name_text
              }
            });
          }

          // 대화 재생.
          await DialoguePublisher.PlayDialogueAsync(dialogue_data, CancellationToken.None);
        }

        // 대화 UI 종료 처리.
        DialoguePublisher.CloseUI();
    }
  }
}