using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    /// <summary>
    /// 전투 공/방 순서
    /// </summary>

    public partial class CombatSystem_Turn : CombatSystem
    {
        
        public enum EnumCombatTurn
        {
            None = 0,

            Attacker, // 공격자 턴
            Defender, // 방어자 턴
        }

        class TurnData
        {
            Int64          m_id;
            EnumCombatTurn m_turn_type;

            int            m_turn_sequence;      // 행동 순서
            int            m_turn_count;         // 행동 횟수
            int            m_turn_count_max;     // 최대 행동 횟수
                           
            int            m_attack_count;       // 공격 횟수
            int            m_attack_count_max;   // 최대 공격 횟수
            int            m_attack_count_extra; // 추가 공격 횟수

            

            public void SetData(Int64 _id, EnumCombatTurn _turn_type, int _turn_sequence, int _turn_count, int _attack_count)
            {
                m_id               = _id;
                m_turn_type        = _turn_type;
                m_turn_sequence    = _turn_sequence;
                m_turn_count_max   = _turn_count;
                m_attack_count_max = _attack_count;
            }

            public Int64 ID           => m_id;
            public int   TurnSequence => m_turn_sequence;   
            public bool  IsAttacker   => m_turn_type == EnumCombatTurn.Attacker;
            public bool  IsDefender   => m_turn_type == EnumCombatTurn.Defender;


            public void  AddTurnSequence(int _value)     => m_turn_sequence      += _value;
            public void  AddMaxTurnCount(int _value)     => m_turn_count_max     += _value;
            public void  AddExtraAttackCount(int _value) => m_attack_count_extra += _value;

            bool IsRemainAttackCount()
            {
                return m_attack_count < (m_attack_count_max + m_attack_count_extra);
            }

            bool HasNextTurn()
            {
                return m_turn_count + 1 < m_turn_count_max;
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
                // 공격 횟수가 남아 있음.
                if (IsRemainAttackCount())
                    return true;

                // 여유 턴이 있는지 체크.
                if (HasNextTurn())
                    return true;

                return false;
            }

            public void ProcessTurn()
            {
                if (!IsRemainAttackCount())
                {
                    // 해당턴의 공격 완료, 다음 턴으로 넘어감
                    ++m_turn_count;
                    ++m_turn_sequence;
                    m_attack_count       = 0;
                    m_attack_count_extra = 0;
                }

                if (IsRemainAttackCount())
                {
                    ++m_attack_count;
                }
            }
        }


        public  EnumCombatTurn CombatTurn   { get; private set; } = EnumCombatTurn.None;
        private TurnData       AttackerData { get; set; }         = new TurnData();
        private TurnData       DefenderData { get; set; }         = new TurnData();

        public CombatSystem_Turn() : base(EnumSystem.CombatSystem_Turn)
        { }

        public bool IsCombatTurn(Int64 _id)
        {
            var turn_data = GetTurnDataBySide(CombatTurn);
            if (turn_data == null)
                return false;

            return turn_data.ID == _id;
        }

        TurnData GetTurnDataByID(Int64 _id)
        {
            if (0 == _id)
                return null;

            if (AttackerData != null && AttackerData.ID == _id)
                return AttackerData;


            if (DefenderData != null && DefenderData.ID == _id)
                return DefenderData;

            return null;
        }

        TurnData GetTurnDataBySide(EnumCombatTurn _side)
        {
            switch (_side)
            {
                case EnumCombatTurn.Attacker: return AttackerData; 
                case EnumCombatTurn.Defender: return DefenderData; 
            }

            return null;
        }

        



        protected override void OnEnter(ICombatSystemParam _param)
        {
            if (_param == null)
                return;

            Reset();

            var attacker = _param.Attacker;
            var defender = _param.Defender;

            // 행동 순서를 계산한다.
            var attacker_turn_sequence = attacker.StatusManager.Buff.Collect(this, attacker, EnumBuffStatus.System_TurnSequence).Calculate(0);
            var defender_turn_sequence = defender.StatusManager.Buff.Collect(this, defender, EnumBuffStatus.System_TurnSequence).Calculate(0);

            // 속도가 특정 값 이상으로 차이가 나면 행동을 2번 합니다.
            const int ADD_EXTRA_TURN_SPEED = 5; 

            var attacker_speed      = attacker.StatusManager.Calc_Speed();
            var defender_speed      = defender.StatusManager.Calc_Speed();
            var attacker_turn_count = (attacker_speed - defender_speed) >= ADD_EXTRA_TURN_SPEED ? 2 : 1;
            var defender_turn_count = (defender_speed - attacker_speed) >= ADD_EXTRA_TURN_SPEED ? 2 : 1;

            // 행동 횟수 관련 버프 적용.
            attacker_turn_count     = attacker.StatusManager.Buff.Collect(this, attacker, EnumBuffStatus.System_TurnCount).Calculate(attacker_turn_count);
            defender_turn_count     = defender.StatusManager.Buff.Collect(this, defender, EnumBuffStatus.System_TurnCount).Calculate(defender_turn_count);

            // 행동당 공격 횟수를 계산합니다. 
            var attacker_attack_count = attacker.StatusManager.Buff.Collect(this, attacker, EnumBuffStatus.System_AttackCount).Calculate(1);
            var defender_attack_count = defender.StatusManager.Buff.Collect(this, defender, EnumBuffStatus.System_AttackCount).Calculate(1);

            AttackerData.SetData(attacker.ID, EnumCombatTurn.Attacker, attacker_turn_sequence, attacker_turn_count, attacker_attack_count);
            DefenderData.SetData(defender.ID, EnumCombatTurn.Defender, defender_turn_sequence, defender_turn_count, defender_attack_count);

            // 공/방 돌입전 스킬 사용할 것이 있다면 이곳에서 사용...!
            attacker.Skill.UseSkill(this, attacker);
            defender.Skill.UseSkill(this, defender);

        }

        /// <summary>
        /// 공/방 공격 순서를 1스텝씩 진행시킨다.
        /// </summary>
        protected override bool OnUpdate(ICombatSystemParam _param)
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
                    CombatTurn = EnumCombatTurn.Attacker;                    
                    return false;
                }
            }
            
            
            if (defender_has_remain_turn)
            {
                // 방어자 턴
                DefenderData.ProcessTurn();
                CombatTurn = EnumCombatTurn.Defender;
                return false;
            }


            // 아무도 행동 할 수 없는 상태. 턴 종료 처리를 진행합시다.
            CombatTurn = EnumCombatTurn.None;
            return true;
        }

        protected override void OnExit(ICombatSystemParam _param)
        {
        }


        public override void Reset()
        {
            CombatTurn = EnumCombatTurn.None;
            AttackerData.Reset();
            DefenderData.Reset();
        }



        public bool AddTurnSequence(EnumCombatTurn _side, int _add_value)
        {
            TurnData turn_data = null;
            switch (_side)
            {
                case EnumCombatTurn.Attacker: turn_data = AttackerData; break;
                case EnumCombatTurn.Defender: turn_data = DefenderData; break;
            }

            if (turn_data != null)
            {
                turn_data.AddTurnSequence(_add_value);
                return true;
            }

            return false;
        }

        public bool AddExtraAttackCount(EnumCombatTurn _side, int _add_value)
        {
            TurnData turn_data = null;
            switch (_side)
            {
                case EnumCombatTurn.Attacker: turn_data = AttackerData; break;
                case EnumCombatTurn.Defender: turn_data = DefenderData; break;
            }

            if (turn_data != null)
            {
                turn_data.AddExtraAttackCount(_add_value);
                return true;
            }

            return false;
        }
    }
}

