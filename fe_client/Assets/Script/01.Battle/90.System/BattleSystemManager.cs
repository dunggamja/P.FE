using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Battle
{
    
    // public static class BattleSystemPauseReason
    // {
    //     public const int Cutscene = 1;
    // }


    public class BattleSystemManager : Singleton<BattleSystemManager>, ISystemManager
    {

        public IBattleSystemParam    Param       { get; private set; }
        public EnumState             State       { get; private set; }                       
        public BattleBlackBoard      BlackBoard  { get; private set; } = new ();


       


        public bool IsProgress => State == EnumState.Progress;
        public bool IsFinished => State == EnumState.Finished;

        private List<BattleSystem>                 m_update_system          = new();
        private int                                m_system_index           = 0;
        
        private Dictionary<int, EnumCommanderType> m_faction_commander      = new ();
        private HashSet<(int, int)>                m_faction_alliance       = new ();

        // private HashSet<int>                       m_pause_reasons          = new();

        // // 승리/패배 조건. (일단 CutsceneCondition 으로 처리...;)
        // private List<CutsceneCondition>            m_conditions_victory = new();
        // private List<CutsceneCondition>            m_conditions_defeat  = new();
        
        
        public CommandQueueHandler           CommandHandler { get; private set; } = new();        
        public MoveRange.VFXHelper_DrawRange DrawRange      { get; private set; } = new();


        public bool                          IsPause 
        {
            get
            {
                // 컷씬이 재생중이면 정지.
                if (CutsceneManager.Instance.IsPlayingCutscene)
                    return true;

                // 아이템 획득 처리중.
                if (BlackBoard.HasValue(EnumBattleBlackBoard.IsInProcess_AcquireItem))
                    return true;

                // 전투가 종료되었을 경우.
                if (BlackBoard.HasValue(EnumBattleBlackBoard.IsBattleFinished))
                    return true;

                return false;
            }
        }


        protected override void Init()
        {
            base.Init();

            
            // 실행할 순서대로 넣어놓는다.

            // 턴 진행
            m_update_system.Add(new BattleSystem_Turn());       
            // 의사 결정
            m_update_system.Add(new BattleSystem_Decision_Making(CommandHandler));    
            // 명령 처리
            m_update_system.Add(new BattleSystem_Command_Progress(CommandHandler));
            // 인벤토리 정리
            m_update_system.Add(new BattleSystem_Inventory());
            // 승리 패배 검사 처리.
            m_update_system.Add(new BattleSystem_Result());




            m_update_system.ForEach(e => e.Init());
            
        }

        public void Release()
        {
            Param = null;
            State = EnumState.None;
            BlackBoard.Reset();

            m_update_system.ForEach(e => e.Release());
            m_system_index = 0;

            m_faction_commander.Clear();
            CommandHandler.Clear();
            
            DrawRange.Clear();
        }

        public void SetData(IBattleSystemParam _param)
        {
            Param = _param;
        }


        void OnEnter()
        {
            m_system_index = 0;
        }

        bool OnUpdate()
        {
            // Update 정지 처리.
            if (IsPause == false)
            {
                // 전투 시작 처리.
                BlackBoard.SetValue(EnumBattleBlackBoard.IsBattleStarted, true);

                // 시스템을 순차적으로 실행해야 한다.
                for(; m_system_index < m_update_system.Count; ++m_system_index)
                {
                    // 진해중인 시스템이 있다면 다음 프레임에 이어서..
                    if (m_update_system[m_system_index].Update(Param) == EnumState.Progress)
                        break;
                } 
                
                
                // LOOP 처리.
                if (m_system_index >= m_update_system.Count)
                {
                    m_system_index  = 0;
                }
            }

            




            // 전투 종료 여부 체크.
            return BlackBoard.HasValue(EnumBattleBlackBoard.IsBattleFinished);
        }


        void OnExit()
        {
        }


        public void Update()
        {
            if (State != EnumState.Progress)
            {
                //State = EnumState.Init;
                State = EnumState.Progress;
                OnEnter();
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

        public ISystem GetSystem(EnumSystem _system_type)
        {
            foreach(var e in m_update_system)
            {
                if (e.SystemType == _system_type)
                    return e;
            }

            Debug.LogError($"Can't Find System, {_system_type.ToString()} in SystemManager[{GetType().ToString()}]");
            return null;
        }

       


        public EnumCommanderType GetFactionCommanderType(int _faction)
        {
            if (m_faction_commander.TryGetValue(_faction, out var command_owner))
                return command_owner;

            return EnumCommanderType.None;
        }

        public void SetFactionCommanderType(int _faction, EnumCommanderType _commander_type)
        {
            if (!m_faction_commander.ContainsKey(_faction))
                 m_faction_commander.Add(_faction, _commander_type);
            else
                 m_faction_commander[_faction] = _commander_type;
        }

        public void SetFactionAlliance(int _faction_1, int _faction_2)
        {
            m_faction_alliance.Add((_faction_1, _faction_2));
            m_faction_alliance.Add((_faction_2, _faction_1));
        }

        public void RemoveFactionAlliance(int _faction_1, int _faction_2)
        {
            m_faction_alliance.Remove((_faction_1, _faction_2));
            m_faction_alliance.Remove((_faction_2, _faction_1));
        }

        public bool IsAlly(int _faction_1, int _faction_2)
        {
            // 같은 진영이면 그냥 아군.
            if (_faction_1 == _faction_2)
                return true;

            
            return m_faction_alliance.Contains((_faction_1, _faction_2));
        }

        public bool IsEnemy(int _faction_1, int _faction_2)
        {
            return IsAlly(_faction_1, _faction_2) == false;
        }

        // public EnumBattleResult Calculate_BattleResult()
        // {
        //     foreach(var condition in m_conditions_victory)
        //     {
        //         // TODO: 음.. condition을 역시 따로 기능을 빼야할지도... null이 들어가니까 어색하군.
        //         if (condition.Verify(null))
        //             return EnumBattleResult.Victory;
        //     }


        //     return EnumBattleResult.None;
        // }



        public BattleSystemManager_IO Save()
        {
            var turn = GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;

            List<(int, EnumCommanderType)> faction_commander = new();
            foreach(var e in m_faction_commander)
                faction_commander.Add((e.Key, e.Value));

            List<(int, int)> faction_alliance = new();
            foreach(var e in m_faction_alliance)
                faction_alliance.Add(e);


            return new BattleSystemManager_IO()
            {
                Turn             = turn?.Save() ?? null,
                BlackBoard       = BlackBoard.Save(),
                FactionCommander = faction_commander,
                FactionAlliance  = faction_alliance
            };
        }

        public void Load(BattleSystemManager_IO _io, bool _is_plan)
        {
            var turn_system = GetSystem(EnumSystem.BattleSystem_Turn) as BattleSystem_Turn;
            turn_system?.Load(_io.Turn);
               
            BlackBoard.Load(_io.BlackBoard);

            m_faction_commander.Clear();
            foreach(var e in _io.FactionCommander)
                m_faction_commander.Add(e.Item1, e.Item2);

            m_faction_alliance.Clear();
            foreach(var e in _io.FactionAlliance)
                m_faction_alliance.Add(e);

            // 나머지는 그냥 초기화 처리.
            CommandHandler.Clear();
            
            if (_is_plan == false)
            {
                DrawRange.Clear();
            }
        }
    }

    public class BattleSystemManager_IO
    {
    public BattleSystem_Turn_IO Turn             { get; set; } = null;
    public BlackBoard_IO        BlackBoard       { get; set; } = null;

    public List<(int, Battle.EnumCommanderType)> 
                                FactionCommander { get; set; } = null;  

    public List<(int, int)>     FactionAlliance  { get; set; } = null;
    }
}



 // public T GetSystem<T>() where T : BattleSystem
        // {
        //     foreach(var e in m_update_system)
        //     {
        //         if (e is T system)
        //             return system;
        //     }

        //     Debug.LogError($"Can't Find System, {typeof(T).ToString()} in SystemManager[{GetType().ToString()}]");
        //     return null;
        // }



        // private EnumState UpdateSystem(EnumSystem _system_type, IBattleSystemParam _param)
        // {
        //     var system = GetSystem(_system_type) as BattleSystem;
        //     if (system != null)
        //         return system.Update(_param);

        //     return EnumState.None;
        // }

        // private EnumState GetSystemState(EnumSystem _system_type)
        // {
        //     var system = GetSystem(_system_type);
        //     if (system != null)
        //         return system.State;

        //     return EnumState.None;
        // }

        // private bool IsSystemFinished(EnumSystem _system_type)
        // {
        //     return GetSystemState(_system_type) == EnumState.Finished;
        // }