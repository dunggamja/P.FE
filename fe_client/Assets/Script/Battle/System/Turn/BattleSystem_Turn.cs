using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    /// <summary>
    /// 전투 공/방 순서
    /// </summary>

    public partial class BattleSystem_Turn : BattleSystem
    {
        public enum EnumTurnSide
        {
            None = 0,

            Attacker,
            Defender,
        }

        class Data
        {
            Int64 m_id;
            bool  m_is_attacker;

            int   m_turn_sequence;
            int   m_turn_count;
            int   m_turn_count_max;
                  
            int   m_attack_count;
            int   m_attack_count_max;
            int   m_attack_count_extra;

            

            public void SetData(Int64 _id, bool _is_attacker, int _turn_sequence, int _turn_count, int _attack_count)
            {
                m_id               = _id;
                m_is_attacker      = _is_attacker;
                m_turn_sequence    = _turn_sequence;
                m_turn_count_max   = _turn_count;
                m_attack_count_max = _attack_count;
            }

            public Int64 ID           => m_id;
            public int   TurnSequence => m_turn_sequence;   // 행동 순서
            public bool  IsAttacker   => m_is_attacker;


            public void  AddTurnSequence(int _value)     => m_turn_sequence      += _value;
            public void  AddMaxTurnCount(int _value)     => m_turn_count_max     += _value;
            public void  AddExtraAttackCount(int _value) => m_attack_count_extra += _value;


            bool IsRemainAttackCount()
            {
                return m_attack_count < (m_attack_count_max + m_attack_count_extra);
            }

            bool IsRemainTurnCount()
            {
                return m_turn_count < m_turn_count_max;
            }

            public void Reset()
            {
                m_id                 = 0;
                m_turn_sequence      = 0;
                m_turn_count         = 0;
                m_turn_count_max     = 0;

                m_attack_count       = 0;
                m_attack_count_max   = 0;
                m_attack_count_extra = 0;
            }

            public bool IsRemainTurn()
            {
                return IsRemainAttackCount() || IsRemainTurnCount();
            }

            public bool ProcessTurn()
            {
                if (!IsRemainAttackCount())
                {
                    if (IsRemainTurnCount())
                    {
                        // 해당턴의 공격 완료, 다음 턴으로 넘어감
                        ++m_turn_count;
                        ++m_turn_sequence;
                        m_attack_count       = 0;
                        m_attack_count_extra = 0;
                    }                    
                }

                if (IsRemainAttackCount())
                {
                    ++m_attack_count;
                    return true;
                }


                // 완료
                return false;
            }
        }


        public  EnumTurnSide CurrentTurn  { get; private set; } = EnumTurnSide.None;
        private Data         AttackerData { get; set; }
        private Data         DefenderData { get; set; }

        public bool IsAttacker(Int64 _id) => AttackerData != null && AttackerData.ID == _id && 0 < _id;
        public bool IsDefender(Int64 _id) => DefenderData != null && DefenderData.ID == _id && 0 < _id;
        public bool IsEngaged(Int64 _id)  => IsAttacker(_id) || IsDefender(_id) ;

        Data GetDataByID(Int64 _id)
        {
            if (0 == _id)
                return null;

            if (AttackerData != null && AttackerData.ID == _id)
                return AttackerData;

            if (DefenderData != null && DefenderData.ID == _id)
                return DefenderData;

            return null;
        }



        protected override void OnEnter(IBattleSystemParam _param)
        {
            if (_param == null)
                return;

            Reset();


            var attacker_status     = _param.Attacker.Status;
            var defender_status     = _param.Defender.Status;

            //var attacker_blackboard = _param.Attacker.BlackBoard;
            //var defender_blackboard = _param.Defender.BlackBoard;

            // 행동 순서를 계산한다.
            var attacker_turn_sequence = attacker_status.Buff.Calculate(attacker_status.OwnerObject, EnumBuffStatus.System_TurnSequence).Calculate(0);
            var defender_turn_sequence = defender_status.Buff.Calculate(defender_status.OwnerObject, EnumBuffStatus.System_TurnSequence).Calculate(0);

            // 속도가 특정 값 이상으로 차이가 나면 행동을 2번 합니다.
            const int ADD_EXTRA_TURN_SPEED = 5; 

            var attacker_speed      = attacker_status.Calc_Speed();
            var defender_speed      = defender_status.Calc_Speed();
            var attacker_turn_count = (attacker_speed - defender_speed) >= ADD_EXTRA_TURN_SPEED ? 2 : 1;
            var defender_turn_count = (defender_speed - attacker_speed) >= ADD_EXTRA_TURN_SPEED ? 2 : 1;

            // 행동 횟수 관련 버프 적용.
            attacker_turn_count     = attacker_status.Buff.Calculate(attacker_status.OwnerObject, EnumBuffStatus.System_TurnCount).Calculate(attacker_turn_count);
            defender_turn_count     = defender_status.Buff.Calculate(defender_status.OwnerObject, EnumBuffStatus.System_TurnCount).Calculate(defender_turn_count);

            // 행동당 공격 횟수를 계산합니다. 
            var attacker_attack_count = attacker_status.Buff.Calculate(attacker_status.OwnerObject, EnumBuffStatus.System_AttackCount).Calculate(1);
            var defender_attack_count = defender_status.Buff.Calculate(defender_status.OwnerObject, EnumBuffStatus.System_AttackCount).Calculate(1);

            AttackerData.SetData(_param.Attacker.ID, _is_attacker:true,  attacker_turn_sequence, attacker_turn_count, attacker_attack_count);
            DefenderData.SetData(_param.Defender.ID, _is_attacker:false, defender_turn_sequence, defender_turn_count, defender_attack_count);

            // 공/방 돌입전 스킬 사용할 것이 있다면 이곳에서 사용...!
            _param.Attacker.Skill.UseSkill(_param.Attacker);
            _param.Defender.Skill.UseSkill(_param.Defender);

        }

        /// <summary>
        /// 공/방 공격 순서를 1스텝씩 진행시킨다.
        /// </summary>
        protected override bool OnUpdate(IBattleSystemParam _param)
        {
            var attacker_has_remain_turn = AttackerData.IsRemainTurn();
            var defender_has_remain_turn = DefenderData.IsRemainTurn();

            if (attacker_has_remain_turn)
            {
                // 1. 공격자의 행동순서 값이 방어자의 값 이하.
                // 2. 방어자에게 남아있는 행동 횟수가 없음.
                var is_attacker_turn = AttackerData.TurnSequence <= DefenderData.TurnSequence;
                if (is_attacker_turn || !defender_has_remain_turn)
                {
                    // 공격자 턴
                    AttackerData.ProcessTurn();
                    CurrentTurn = EnumTurnSide.Attacker;
                    return false;
                }
            }
            
            
            if (defender_has_remain_turn)
            {
                // 방어자 턴
                DefenderData.ProcessTurn();
                CurrentTurn = EnumTurnSide.Defender;
                return false;
            }


            // 아무도 행동 할 수 없는 상태. 턴 종료 처리를 진행합시다.
            CurrentTurn = EnumTurnSide.None;
            return true;
        }

        protected override void OnExit(IBattleSystemParam _param)
        {
        }


        public void Reset()
        {
            CurrentTurn = EnumTurnSide.None;
            AttackerData.Reset();
            DefenderData.Reset();
        }



        public bool AddTurnSequence(EnumTurnSide _side, int _add_value)
        {
            Data turn_data = null;
            if      (_side == EnumTurnSide.Attacker) turn_data = AttackerData;
            else if (_side == EnumTurnSide.Defender) turn_data = DefenderData;

            if (turn_data != null)
            {
                turn_data.AddTurnSequence(_add_value);
                return true;
            }

            return false;
        }

        public bool AddExtraAttackCount(EnumTurnSide _side, int _add_value)
        {
            Data turn_data = null;
            if (_side == EnumTurnSide.Attacker) turn_data = AttackerData;
            else if (_side == EnumTurnSide.Defender) turn_data = DefenderData;

            if (turn_data != null)
            {
                turn_data.AddExtraAttackCount(_add_value);
                return true;
            }

            return false;
        }

    }
}

