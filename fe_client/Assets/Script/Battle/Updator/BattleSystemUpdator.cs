using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


namespace Battle
{

    public class BattleSystemUpdator : SingletonMono<BattleSystemUpdator>
    {

        public bool IsInitialized { get; private set; } = false;

        protected override float LoopInterval => Constants.BATTLE_SYSTEM_UPDATE_INTERVAL;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IsInitialized = false;

            // �������ϸ��� ���� �ӽ� ó��
            // Debug.unityLogger.logEnabled = false;

            // DOTween �α� ���� ���� - Init()�� ����Ͽ� �ùٸ��� ����
            DOTween.Init(false, false, LogBehaviour.ErrorsOnly);


            // ���� ������ �ʱ�ȭ.
            Initialize_GameData().Forget();
        }

        

        protected override void OnLoop()
        {
            base.OnLoop();

            if (IsInitialized == false)
                return;

            BattleSystemManager.Instance.Update();
        }

        // // Start is called before the first frame update
        // protected override void Start()
        // {
        //     base.Start();
        // }

        async UniTask Initialize_GameData()
        {
            // ��Ʈ ������ �ε�.
            await DataManager.Instance.LoadSheetData();

            // ���� �ð��� ������� �õ尪 �ʱ�ȭ
            Util.SetRandomSeed((int)System.DateTime.Now.Ticks);

            // ���� �ý��� �ʱ�ȭ
            Test_BattleSystem_Setup_Terrain();

            // ���� ���� & �ɷ�ġ ����.
            Test_BattleSystem_Setup_Unit();

            IsInitialized = true;
        }



        void Test_BattleSystem_Setup_Terrain()
        {
            var terrain_map = new TerrainMap();
            terrain_map.Initialize(100, 100);
            for(int y = 0; y < terrain_map.Height; ++y)
            {
                for(int x = 0; x < terrain_map.Width; ++x)
                {
                    terrain_map.Attribute.SetAttribute(x, y, EnumTerrainAttribute.Ground);
                }
            }

            TerrainMapManager.Instance.SetTerrainMap(terrain_map);
        }


        void Test_BattleSystem_Setup_Unit()
        {
            var attacker_id  = Util.GenerateID();
            var attacker_id2 = Util.GenerateID();
            var defender_id  = Util.GenerateID();
            var defender_id2 = Util.GenerateID();
            var attacker     = Entity.Create(attacker_id);
            var attacker_2   = Entity.Create(attacker_id2);
            var defender     = Entity.Create(defender_id);
            var defender_2   = Entity.Create(defender_id2);


            EntityManager.Instance.AddEntity(attacker);
            EntityManager.Instance.AddEntity(attacker_2);
            EntityManager.Instance.AddEntity(defender);
            EntityManager.Instance.AddEntity(defender_2);

            // 1. �÷��̾� ����, 2: AI ����.
            attacker.SetFaction(1);
            attacker_2.SetFaction(1);
            defender.SetFaction(2);
            defender_2.SetFaction(2);


            // 2. AI Ÿ�� ����.
            attacker.SetAIType(EnumAIType.Attack);
            attacker_2.SetAIType(EnumAIType.Attack);
            defender.SetAIType(EnumAIType.Attack);
            defender_2.SetAIType(EnumAIType.Attack);
            
            BattleSystemManager.Instance.SetFactionCommanderType(1, EnumCommanderType.Player);
            BattleSystemManager.Instance.SetFactionCommanderType(2, EnumCommanderType.AI);


            var list_attacker = new []{ attacker, attacker_2 };
            var list_defender = new []{ defender, defender_2 };


            foreach(var e in list_attacker)
            {
                var attacker_status = e.StatusManager.Status as UnitStatus;

                // �ɷ�ġ ����.
                attacker_status.SetPoint(EnumUnitPoint.HP, 22);
                attacker_status.SetPoint(EnumUnitPoint.HP_Max, 22);
                attacker_status.SetStatus(EnumUnitStatus.Strength, 6);
                attacker_status.SetStatus(EnumUnitStatus.Skill, 5);
                attacker_status.SetStatus(EnumUnitStatus.Speed, 7);
                attacker_status.SetStatus(EnumUnitStatus.Movement, 4);
                attacker_status.SetStatus(EnumUnitStatus.Defense, 5);
                attacker_status.SetStatus(EnumUnitStatus.Resistance, 3);
                attacker_status.SetStatus(EnumUnitStatus.Luck, 5);
                attacker_status.SetStatus(EnumUnitStatus.Weight, 4);

                // �׶��� ����
                e.SetPathAttribute(EnumPathOwnerAttribute.Ground);

                // ���� ����.
                var attacker_weapon = Item.Create(Util.GenerateID(), Data_Const.KIND_WEAPON_SWORD_IRON);
                e.Inventory.AddItem(attacker_weapon);
                e.StatusManager.Weapon.Equip(attacker_weapon.ID);
            }

            {
                // ų�ҵ� ���.
                var attacker_weapon = Item.Create(Util.GenerateID(), Data_Const.KIND_WEAPON_SWORD_KILL);
                attacker_2.Inventory.AddItem(attacker_weapon);
                attacker_2.StatusManager.Weapon.Equip(attacker_weapon.ID);
            }

            foreach(var e in list_defender)
            {
                // �ɷ�ġ ����.
                var defender_status = e.StatusManager.Status as UnitStatus;                
                defender_status.SetPoint(EnumUnitPoint.HP, 22);
                defender_status.SetPoint(EnumUnitPoint.HP_Max, 22);
                defender_status.SetStatus(EnumUnitStatus.Strength, 6);
                defender_status.SetStatus(EnumUnitStatus.Skill, 5);
                defender_status.SetStatus(EnumUnitStatus.Speed, 7);
                defender_status.SetStatus(EnumUnitStatus.Movement, 4);
                defender_status.SetStatus(EnumUnitStatus.Defense, 5);
                defender_status.SetStatus(EnumUnitStatus.Resistance, 3);
                defender_status.SetStatus(EnumUnitStatus.Luck, 5);
                defender_status.SetStatus(EnumUnitStatus.Weight, 4);

                // �׶��� ����.
                e.SetPathAttribute(EnumPathOwnerAttribute.Ground);

                // ���� ����.
                var defender_weapon = Item.Create(Util.GenerateID(), Data_Const.KIND_WEAPON_SWORD_KILL);
                e.Inventory.AddItem(defender_weapon);
                e.StatusManager.Weapon.Equip(defender_weapon.ID);
            }

            // ���� ��ġ ����.
            {
                attacker.UpdateCellPosition(
                    (2, 0), 
                    _visual_immediatly: true, 
                    _is_plan: false);

                attacker_2.UpdateCellPosition(
                    (3, 0), 
                    _visual_immediatly: true, 
                    _is_plan: false);

                defender.UpdateCellPosition(
                    (2, 4), 
                    _visual_immediatly: true, 
                    _is_plan: false);

                defender_2.UpdateCellPosition(
                    (3, 3),  
                    _visual_immediatly: true, 
                    _is_plan: false);
            }

            // ���� ������Ʈ ����.
            attacker.CreateProcess();
            attacker_2.CreateProcess();
            defender.CreateProcess();
            defender_2.CreateProcess();
        }



    }
}