using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{


    public static class CombatHelper
    {
       
        public struct Result_Action
        {
            public bool isAttacker;
            public int  Damage;

            public static Result_Action Create(bool _isAttacker, int _damage)
            {
                return new Result_Action()
                {
                    isAttacker = _isAttacker,
                    Damage     = _damage
                };
            }

        }
        public class Result_Unit
        {
            public Int64           EntityID     { get; set; }
            public Int64           WeaponID     { get; set; }
            public int             Damage       { get; set; }
            public int             HitRate      { get; set; }
            public int             CriticalRate { get; set; }
            public int             HP_Before    { get; set; }
            public int             HP_After     { get; set; }
        }

        public class Result
        {
            public Result_Unit         Attacker { get; set; } = new();
            public Result_Unit         Defender { get; set; } = new();
            public List<Result_Action> Actions  { get; set; } = new();
        }


        // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
        public static Result Run_Plan(
            Int64               _attacker_id,
            Int64               _target_id,
            Int64               _weapon_id,
            EnumUnitCommandType _command_type,
            (int x, int y)      _attack_position)
        {
            
            var attacker     = EntityManager.Instance.GetEntity(_attacker_id);
            var target       = EntityManager.Instance.GetEntity(_target_id);  

            if (attacker == null || target == null)
                return null;

            // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
            var snapshot = GameSnapshot.Save(); 

            try
            {
                // 공격자 무기 장착. 실패시 종료 처리.
                if (attacker.ProcessAction(attacker.Inventory.GetItem(_weapon_id), EnumItemActionType.Equip) == false)
                    return null;


                var result = new Result();

                // 공격자 위치 업데이트.
                attacker.UpdateCellPosition(
                    _cell:          _attack_position,
                    _visual_update: (_apply: false, _immediatly: false),
                    _is_plan:       false);

                // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                {
                    // var combat_param = //ObjectPool<CombatParam_Plan>
                    var combat_param = new CombatParam_Plan().Set(attacker, target, _command_type);

                    // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                    result.Attacker.EntityID  = attacker.ID;
                    result.Attacker.WeaponID  = _weapon_id;
                    result.Attacker.HP_Before = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                    result.Defender.EntityID  = target.ID;
                    result.Defender.WeaponID  = target.StatusManager.Weapon.ItemID;
                    result.Defender.HP_Before = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);


                    // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                    {
                        CombatSystemManager.Instance.Setup(combat_param);
                        while (CombatSystemManager.Instance.IsFinished == false)
                            CombatSystemManager.Instance.Update();
                    }

                    // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                    var damage_result = CombatSystemManager.Instance.GetCombatDamageResult();
                    foreach (var damage in damage_result)
                    {
                        // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                        var is_attacker  = (damage.AttackerID == attacker.ID);

                        // 공격자와 타겟 데미지 저장. (공격자, 타겟, 데미지)
                        var unit_info          = (is_attacker) ? result.Attacker: result.Defender;
                        unit_info.Damage       = Math.Max(damage.Result_Damage, unit_info.Damage);
                        unit_info.HitRate      = Math.Max(damage.Result_HitRate_Percent, unit_info.HitRate);
                        unit_info.CriticalRate = Math.Max(damage.Result_CriticalRate_Percent, unit_info.CriticalRate);


                        // 공격자와 타겟 데미지 저장.
                        var damage_value = damage.Result_Damage;

                        result.Actions.Add(Result_Action.Create(is_attacker, damage_value));
                    }

                    // 공격자와 타겟 HP 상태 저장.
                    result.Attacker.HP_After     = attacker.StatusManager.Status.GetPoint(EnumUnitPoint.HP);
                    result.Defender.HP_After     = target.StatusManager.Status.GetPoint(EnumUnitPoint.HP);

                    // ObjectPool<CombatParam_Plan>.Return(combat_param);
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"CombatHelper: Run_Plan failed. {_attacker_id} -> {_target_id}, e:{e.Message}");
                return null;
            }
            finally
            {
                // 공격 후 상태 복구.
                GameSnapshot.Load(snapshot, _is_plan: true);
            }
        }
    

        public static bool IsAlly(Int64 _attacker_id, Int64 _target_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            // TODO: 일단 FixedObject 제외.
            if (target.IsFixedObject)
                return false;

            

            // 공격자와 타겟이 같은 진영인지 체크.
            var    is_ally  = BattleSystemManager.Instance.IsFactionAlliance(attacker.GetFaction(), target.GetFaction());


            // TODO: 혼란등 걸려있을때는 따로 체크해야 할듯하군.
            {
                // is_ally = rand() %2 ?;;;
                // is_ally = !is_ally;;;;
            }

            return is_ally == true;
        }
    
    
        public static bool IsEnemy(Int64 _attacker_id, Int64 _target_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            // TODO: 일단 FixedObject 제외.
            if (target.IsFixedObject)
                return false;

                      
            // 공격자와 타겟이 같은 진영인지 체크.
            var    is_ally  = BattleSystemManager.Instance.IsFactionAlliance(attacker.GetFaction(), target.GetFaction());

            // TODO: 혼란등 걸려있을때는 따로 체크해야 할듯하군.
            {
                // is_ally = rand() %2 ?;;;
                // is_ally = !is_ally;;;;
            }

            return is_ally == false;
        }

        public static bool IsExchangeable(Int64 _attacker_id, Int64 _target_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            // 동일한 진영인지 체크합니다.
            return attacker.GetFaction() == target.GetFaction();
        }
        

        public static bool IsTargetable(Int64 _attacker_id, Int64 _target_id, Int64 _weapon_id)
        {
            if (_attacker_id <= 0 || _target_id <= 0 || _weapon_id <= 0)
                return false;

            var attacker = EntityManager.Instance.GetEntity(_attacker_id);
            var target   = EntityManager.Instance.GetEntity(_target_id);

            if (attacker == null || target == null)
                return false;

            var item = attacker.Inventory.GetItem(_weapon_id);
            if (item == null)
                return false;

            var target_type =  ItemHelper.GetItemTargetType(item.Kind);
            if (target_type == EnumTargetType.None)
                return false;


            switch(target_type)
            {
                case EnumTargetType.Owner: return _attacker_id == _target_id;
                case EnumTargetType.Ally : return _attacker_id != _target_id && IsAlly(_attacker_id, _target_id);
                case EnumTargetType.Enemy: return _attacker_id != _target_id && IsEnemy(_attacker_id, _target_id);
            }          

            return false;
        }


        public static bool FindExchangeTargetList(Int64 _entity_id, List<Int64> _target_list)
        {
            if (_entity_id <= 0 || _target_list == null)
                return false;


            _target_list.Clear();

            var entity = EntityManager.Instance.GetEntity(_entity_id);
            if (entity == null)
                return false;

            // 공격 범위 탐색.
            using var attack_range_visit = ObjectPool<Battle.MoveRange.AttackRangeVisitor>.AcquireWrapper();
            attack_range_visit.Value.SetData(
                _draw_flag:         (int)Battle.MoveRange.EnumDrawFlag.ExchangeRange,
                _terrain:           TerrainMapManager.Instance.TerrainMap,
                _entity_object:     entity,
                _use_base_position: false,
                _use_weapon_id:     0
            );

            PathAlgorithm.FloodFill(attack_range_visit.Value);


            // 타겟 목록 순회.
            foreach(var pos in attack_range_visit.Value.Visit_Exchange)
            {
                var target_id = TerrainMapManager.Instance.TerrainMap.EntityManager.GetCellData(pos.x, pos.y);
                if (target_id > 0)
                {   
                    // 공격 가능한 타겟 찾기.
                    if (CombatHelper.IsExchangeable(_entity_id, target_id) == false)
                        continue;             

                    _target_list.Add(target_id);
                }
            }

            return _target_list.Count > 0;
        }


        public static bool FindWeaponTargetableList(bool _is_wand,Int64 _entity_id, Int64 _weapon_id, List<Int64> _target_list)
        {
            if (_entity_id <= 0 || _weapon_id <= 0 || _target_list == null)
                return false;

            _target_list.Clear();

            var entity = EntityManager.Instance.GetEntity(_entity_id);
            if (entity == null)
                return false;

            var draw_flag   = (_is_wand) 
                            ? (int)Battle.MoveRange.EnumDrawFlag.WandRange 
                            : (int)Battle.MoveRange.EnumDrawFlag.AttackRange;

            // 공격 범위 탐색.
            using var attack_range_visit = ObjectPool<Battle.MoveRange.AttackRangeVisitor>.AcquireWrapper();
            attack_range_visit.Value.SetData(
                _draw_flag:         draw_flag,
                _terrain:           TerrainMapManager.Instance.TerrainMap,
                _entity_object:     entity,
                _use_base_position: false,
                _use_weapon_id:     _weapon_id
            );

            PathAlgorithm.FloodFill(attack_range_visit.Value);


            // 타겟 목록 순회.
            var target_list = (_is_wand) 
                            ? attack_range_visit.Value.Visit_Wand
                            : attack_range_visit.Value.Visit_Weapon;

            foreach(var pos in target_list)
            {
                var target_id = TerrainMapManager.Instance.TerrainMap.EntityManager.GetCellData(pos.x, pos.y);
                if (target_id > 0)
                {   
                    // 공격 가능한 타겟 찾기.
                    if (CombatHelper.IsTargetable(_entity_id, target_id, _weapon_id) == false)
                        continue;             

                    _target_list.Add(target_id);
                }
            }

            return _target_list.Count > 0;
        }
    }
}