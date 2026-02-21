using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Entity : 
     IOwner,
     IFaction,
     IEventReceiver,
     IPathOwner
    //  ICommand,
    {
        public Int64                ID              { get; private set; }
        // public bool                 IsFixedObject   { get; private set; } = false;

        public (int x, int y)       Cell            { get; private set; }
        public (int x, int y)       Cell_Prev       { get; private set; }
        public bool                 Cell_Occupied   { get; private set; } = false;
        public bool                 HasZOC_Last     { get; private set; } = false;
        

        public EntityBlackBoard     BlackBoard      { get; }
        public BattleSkill          Skill           { get; } 
        public StatusManager        StatusManager   { get; } 
        public Inventory            Inventory       { get; }
        public PathNodeManager      PathNodeManager { get; } 
        public PathVehicle          PathVehicle     { get; } 
        public (int x, int y)       PathBasePosition { get; private set; } 
        public int                  PathTerrainKind  
        {
            get
            {
                var class_kind = StatusManager.Status.ClassKIND;
                var mounted    = BlackBoard.HasValue(EnumEntityBlackBoard.Mounted);
                return DataManager.Instance.UnitSheet.GetTerrainKind(class_kind, mounted);
            }
        }

        public EnumUnitMountedType  PathMountedType => DataManager.Instance.UnitSheet.GetClassMountedType(ClassKind);
        public bool                 PathMounted     => BlackBoard.HasValue(EnumEntityBlackBoard.Mounted);

        // 비병이 탑승중일 경우 ZOC 체크를 하지 않습니다.
        public bool                 PathHasZOC      => (PathMounted == false) || (PathMountedType != EnumUnitMountedType.Flyer);
        public int                  PathZOCFaction  => GetFaction();
        public int                  PathMoveRange   => (PathMounted) 
                ? StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement_Mounted)
                : StatusManager.GetBuffedUnitStatus(EnumUnitStatus.Movement);
                
        public AIManager            AIManager       { get; }
        public AIDataManager        AIDataManager   { get; }
        public CommandManager       CommandManager  { get; private set; }    


        public int     ClassKind    => StatusManager.Status.ClassKIND;


        public bool    IsDead       => StatusManager.Status.GetPoint(EnumUnitPoint.HP) <= 0;

        public string  AssetName  { get; private set; } = string.Empty;


        private Dictionary<EnumEntityHUD, Int64> m_hud_list = new();



        protected Entity(Int64 _id)
        {
            ID                = _id;
            // IsFixedObject     = _is_fixed_object;
            Cell              = (0, 0);
            Cell_Prev         = Cell;
            Cell_Occupied     = false;
            PathBasePosition  = Cell;
                 
            BlackBoard        = new EntityBlackBoard();
            Skill             = new BattleSkill();
            StatusManager     = new StatusManager(this);
            PathNodeManager   = new PathNodeManager();
            PathVehicle       = new PathVehicle_Basic();
            Inventory         = new Inventory();
            AIManager         = new AIManager();            
            AIDataManager     = new AIDataManager();
            CommandManager    = new CommandManager();

        }

        public static Entity Create(Int64 _id)
        {
            var new_entity = new Entity(_id);
            new_entity.Initialize();

            return new_entity;
        }

        public void Initialize()
        {
            Inventory.Initialize(this);
            AIDataManager.Initialize(this);
            AIManager.Initialize(AIDataManager);
            CommandManager.Initialize(this);

            EventDispatchManager.Instance.AttachReceiver(this);   
        }

        public void Reset()
        {
            // 좌표 초기화. 
            UpdateCellOccupied(false);

            // HUD 삭제.
            DeleteHUD();


            EventDispatchManager.Instance.DetachReceiver(this);
        }

        public void CreateHUD()
        {
            CreateHUD_HP();
        }

        public void DeleteHUD()
        {
            // HUD 삭제.
            foreach((var _, var hud_id) in m_hud_list)
            {
                GUIManager.Instance.CloseUI(hud_id);
            }

            // 
            m_hud_list.Clear();
        }


        public int GetFaction()
        {
            return (int)BlackBoard.GetValue(EnumEntityBlackBoard.Faction);
        }

        public void SetFaction(int _faction)
        {
            BlackBoard.SetValue(EnumEntityBlackBoard.Faction, _faction);
        }

        public void SetAIType(EnumAIType _ai_type)
        {
            AIDataManager.SetAITypeBase(_ai_type);
        }

        public void SetAssetName(string _asset_name)
        {
            AssetName = _asset_name;
        }


        public void Process_OnCreate()
        {
            // 태그 셋팅 처리.
            TagHelper.SetupTag_Entity_Faction(this);

            // HUD 생성 처리.
            CreateHUD();
        }

        public void Process_OnDelete()
        {
            // 태그 삭제 처리.
            TagHelper.RemoveTag_Entity_Faction(this);

            // Reset()
            Reset();
        }

        public Entity_IO Save()
        {
            return new Entity_IO()
            {
                ID               = ID,
                // IsFixedObject    = IsFixedObject,

                Cell             = Cell,
                Cell_Prev        = Cell_Prev,
                Cell_Occupied    = Cell_Occupied,

                BlackBoard       = BlackBoard.Save(),
                Skill            = Skill.Save(),
                StatusManager    = StatusManager.Save(),
                Inventory        = Inventory.Save(),


                PathVehicle      = PathVehicle.Save(),
                PathBasePosition = PathBasePosition,
                // PathNodeManager  = PathNodeManager.Save(), 
                // PathAttribute    = PathAttribute,

                AIDataManager    = AIDataManager.Save(),
                // AIManager        = AIManager.Save(),
                // CommandManager   = CommandManager.Save(),                
                // AssetName        = AssetName,
            };
        }

        public void Load(Entity_IO _snapshot)
        {
            ID            = _snapshot.ID;
            // IsFixedObject = _snapshot.IsFixedObject;
            Cell          = _snapshot.Cell;
            Cell_Prev     = _snapshot.Cell_Prev;
            Cell_Occupied = _snapshot.Cell_Occupied;

            BlackBoard.Load(_snapshot.BlackBoard);
            Skill.Load(_snapshot.Skill);
            StatusManager.Load(_snapshot.StatusManager);
            Inventory.Load(_snapshot.Inventory);

            PathVehicle.Load(_snapshot.PathVehicle);
            PathBasePosition = _snapshot.PathBasePosition;

            AIDataManager.Load(_snapshot.AIDataManager);

        }
    }



    public class Entity_IO
    {
        public Int64               ID               { get; set; } = 0;
        // public bool                IsFixedObject    { get; set; } = false;
        public (int x, int y)      Cell             { get; set; } = (0, 0);
        public (int x, int y)      Cell_Prev        { get; set; } = (0, 0);
        public bool                Cell_Occupied    { get; set; } = false;
    
        public BlackBoard_IO       BlackBoard       { get; set; } = new();
        public BattleSkill_IO      Skill            { get; set; } = new();
        public StatusManager_IO    StatusManager    { get; set; } = new();
        public Inventory_IO        Inventory        { get; set; } = new();

        public PathVehicle_IO      PathVehicle      { get; set; } = new();
        public (int x, int y)      PathBasePosition { get; set; } = (0, 0);



        // public AIManager_IO      AIManager        { get; set; } = new();
        public AIDataManager_IO  AIDataManager    { get; set; } = new();
        // public CommandManager_IO CommandManager   { get; set; } = new();
        // public string            AssetName        { get; set; } = string.Empty;

    }


}

