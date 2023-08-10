using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EnumState
    {
        None,
        Enter,
        Progress,
        Finished,
    }

    public interface IBattleSystemParam
    {
        BattleObject Attacker { get; set; }
        BattleObject Defender { get; set; }
    }

    public class BattleSystem 
    {
        public EnumState State { get; private set; } = EnumState.None;

        public bool IsEnter    => State == EnumState.Enter;
        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;

        protected virtual  void OnEnter(IBattleSystemParam _param)  { }
        protected virtual  bool OnUpdate(IBattleSystemParam _param) { return true; }
        protected virtual  void OnExit(IBattleSystemParam _param)   { }

        public void Update(IBattleSystemParam _param)
        {
            if (State != EnumState.Progress)
            {
                State = EnumState.Enter;

                OnEnter(_param);

                State = EnumState.Progress;
            }

            if (OnUpdate(_param))
            {
                State = EnumState.Finished;
            }

            if (State != EnumState.Progress)
            {
                OnExit(_param);
            }
        }
    }


    /// <summary>
    /// SRPG에서 유닛끼리 공방을 주고받는 부분을 관리합니다.
    /// ex) 공격자 한방 수비자 한방 씩 때리는 부분
    /// </summary>
    public class BattleSystemManager : Singleton<BattleSystemManager>
    {
        public IBattleSystemParam Param      { get; private set; }
        public EnumState          State      { get; private set; }
        public BattleSystem_Turn  TurnSystem { get; private set; }




        public void Reset()
        {
            Param = null;
            State = EnumState.None;
            TurnSystem.Reset();
        }

        public void SetData(IBattleSystemParam _param)
        {
            Param = _param;
        }

        void OnEnter()
        {
        }

        bool OnUpdate()
        {
            // 턴 순서 정하기
            TurnSystem.Update(Param);

            // TODO: 턴이 정해졌으면 데미지 처리.
            // DamageSystem.Update(Param)


            // 턴이 모두 종료됨
            if (TurnSystem.IsFinished)
                return true;

            //// 누군가 죽었음.
            //if (Param.Attacker.IsDead || Param.Defender.IsDead)
            //    return true;

            return false;
        }


        void OnExit()
        {
        }


        void Update()
        {
            if (State != EnumState.Progress)
            {
                State = EnumState.Enter;

                OnEnter();

                State = EnumState.Progress;
            }

            if (OnUpdate())
            {
                State = EnumState.Finished;
            }

            if (State != EnumState.Progress)
            {
                OnExit();
            }
        }
    }
}



