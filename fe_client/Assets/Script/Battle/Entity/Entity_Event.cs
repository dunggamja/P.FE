using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [EventReceiver(
        typeof(Battle_Situation_UpdateEvent),
        typeof(Battle_Item_UpdateEvent)
        )]
    public partial class Entity
    {

        public void OnReceiveEvent(IEventParam _event)
        {            
            switch (_event)
            {
                case Battle_Situation_UpdateEvent situation_updated:
                    // 상황 갱신.
                    OnReceiveEvent_Battle_Situation_UpdateEvent(situation_updated);
                    break;

                case Battle_Item_UpdateEvent item_updated:
                    // 아이템 갱신.
                    OnReceiveEvent_Battle_Item_UpdateEvent(item_updated);
                    break;
            }
        }

        private void OnReceiveEvent_Battle_Item_UpdateEvent(Battle_Item_UpdateEvent _event)
        {
            if (_event == null)
                return;

            if (_event.EntityID != ID)
                return;

            // 버프 적용.
            ApplyBuff_Item(_event.ItemKind, _event.ActionType);

            // 그외 처리.
        }

        void OnReceiveEvent_Battle_Situation_UpdateEvent(Battle_Situation_UpdateEvent _event)
        {
            if (_event == null)
                return;



            if (_event.Situation == EnumSituationType.BattleSystem_Turn_Changed)
            {
                // 턴 변경 시 모든 명령 가능 상태로 변경.
                SetAllCommandEnable();
            }


            // 스킬 사용.
            Skill.UseSkill(_event.Situation, this);
        }   


        void ApplyBuff_Inventory()
        {
            // 소지, 장비한 아이템의 버프 제거.
            {
                using var list_buff_id = ListPool<Int64>.AcquireWrapper();
                StatusManager.Buff.Collect_BuffID_ByContentsType(EnumBuffContentsType.Item_Equipment, list_buff_id.Value);
                StatusManager.Buff.Collect_BuffID_ByContentsType(EnumBuffContentsType.Item_Accessory, list_buff_id.Value);
                foreach(var buff_id in list_buff_id.Value)
                {
                    StatusManager.Buff.RemoveBuff(buff_id);
                }
            }

            // 소지한 아이템의 버프 적용.
            {
                using var list_item    = ListPool<Item>.AcquireWrapper();
                Inventory.CollectItem(list_item.Value);
                
                foreach(var item in list_item.Value)
                {
                    ApplyBuff_Item(item.Kind, EnumItemActionType.Acquire);
                }
            }

            // 장비한 아이템의 버프 적용.
            {
                if (0 < StatusManager.Weapon.ItemID)
                {
                    var item_object = StatusManager.Weapon.ItemObject;
                    if (item_object != null)
                    {
                        ApplyBuff_Item(item_object.Kind, EnumItemActionType.Equip);
                    }
                }
            }
        }

        void ApplyBuff_Item(Int32 _item_kind, EnumItemActionType _action_type)
        {
            // 아이템 타입에 따라서 버프 처리 방식이 다르다.
            var sheet_item = DataManager.Instance.ItemSheet.GetStatus(_item_kind);
            if (sheet_item == null)
                return;
            
            // 버프 갱신을 하는 상황인지 확인.
            var update_buff = BuffHelper.IsBuffUpdateSituation((EnumItemType)sheet_item.TYPE, _action_type);
            if (update_buff == false)
                return;

            // 처리할 버프가 있는지 확인.
            using var list_buff_id = ListPool<Int64>.AcquireWrapper();
            Item.CollectBuffID(_item_kind, list_buff_id.Value);
            if (list_buff_id.Value.Count == 0)
                return;

            var is_add_buff        = BuffHelper.IsAddBuff(_action_type);
            var buff_contents_type = BuffHelper.GetContentsTypeByItemType((EnumItemType)sheet_item.TYPE);
            
                
            foreach (var buff_id in list_buff_id.Value)
            {
                if (is_add_buff)
                    StatusManager.Buff.AddBuff(Buff.CreateBuff((int)buff_id, buff_contents_type));
                else
                    StatusManager.Buff.RemoveBuff(buff_id);
            }
        }

        void ApplyConsume_Item(Int32 _item_kind)
        {
            
            var heal_value = ItemHelper.Calculate_Item_Heal(_item_kind, this);
            if (heal_value > 0)
                ApplyHeal(heal_value);
        }


        // void OnReceiveEvent_Battle_AI_Command_DecisionEvent(Battle_AI_Command_DecisionEvent _event)
        // {
        //     // �׾����� �ϰ͵� ����.
        //     if (IsDead)
        //         return;

        //     // ������ �ٸ�.
        //     if (_event.Faction != GetFaction())
        //         return;

        //     // �ൿ �켱������ �ٸ�.
        //     if (_event.Priority != GetCommandPriority())
        //         return;
            
        //     // TODO: ���߿� �ʿ��� Sensor�� ������Ʈ �� �� �ְ� ���� �ʿ�.
        //     AIManager.Update(this);

        //     // event param�� ���.
        //     _event.TrySetScore(ID, GetAIScoreMax().score);
        // }

        // void OnReceiveEvent_Battle_Entity_MoveEvent(Battle_Entity_MoveEvent _event)
        // {
        //     if (_event == null)
        //         return;
        // }
    }
}