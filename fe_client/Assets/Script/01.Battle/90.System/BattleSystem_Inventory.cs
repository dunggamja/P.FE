using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    


    public class BattleSystem_Inventory : BattleSystem
    {
        private Queue<Int64> m_full_inventory_entities = new();
        private UniTask      m_task;



        public BattleSystem_Inventory() : base(EnumSystem.BattleSystem_Inventory)
        {
        }

        protected override void OnInit()
        {
            // throw new NotImplementedException();
        }

        protected override void OnRelease()
        {
            // throw new NotImplementedException();
        }

        protected override void OnEnter(IBattleSystemParam _param)
        {
            m_full_inventory_entities.Clear();

            EntityManager.Instance.Loop(e =>
            {
                if (e == null || e.Inventory == null)
                    return;

                // 플레이어 캐릭터만 체크합니다.
                var faction         = e.GetFaction();
                var commander_type  = BattleSystemManager.Instance.GetFactionCommanderType(faction);
                if (commander_type != EnumCommanderType.Player)
                    return;

                // 인벤토리가 꽉찬 경우, 체크합니다.
                if (e.Inventory.IsFull)
                    m_full_inventory_entities.Enqueue(e.ID);
            });

            m_task = UniTask.CompletedTask;
        }

        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            // 처리해야 할 entity_ID
            if (m_full_inventory_entities.Count > 0)
            {
                m_task = Process_DiscardItem(m_full_inventory_entities.Dequeue());
            }

            // 처리중인 작업이 있는지 체크.
            if (m_task.Status == UniTaskStatus.Pending)
                return false;
                

            // 처리할 entity_ID가 없으면 종료.
            return  (m_full_inventory_entities.Count == 0);
            
            
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
            // throw new NotImplementedException();
        }


        async UniTask Process_DiscardItem(Int64 _entity_id)
        {
            // TODO: 인벤토리 관련 유니태스크 처리 진행.


            
        }

    }
}
