using System;
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

            // 디버그 로그 비활성화
            // Debug.unityLogger.logEnabled = false;

            // DOTween 초기화 - Init()에서 초기화 되었으면 비활성화
            DOTween.Init(false, false, LogBehaviour.ErrorsOnly);


            // 게임 데이터 초기화.
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
            // 게임 데이터 로드.
            await DataManager.Instance.LoadSheetData();

            // 랜덤 시드 설정.
            Util.SetRandomSeed((int)System.DateTime.Now.Ticks);

            // 맵 파일 로드.
            var (map_file, map_setting, faction_setting) = await Load_Map_File(AssetName.DEMO_MAP_FILE);
            if  (map_file == null || map_setting == null || faction_setting == null)
            {
                Debug.LogError("Failed to load map file");
                return;
            }

            map_setting.Initialize();


            // 지형 맵 셋팅.
            Test_BattleSystem_Setup_Terrain(map_file);

            // 유닛 셋팅.
            Test_BattleSystem_Setup_Unit(map_file, map_setting);

            // 진영 셋팅.
            Test_BattleSystem_Setup_Faction(faction_setting);

            // 태그 셋팅.
            Test_BattleSystem_Setup_Tag(map_setting);


            IsInitialized = true;
        }


        async UniTask<(MapBakeData, sheet_map_setting, sheet_map_faction_setting)> Load_Map_File(string _asset_name)
        {
            var map_file = await AssetManager.Instance.LoadAssetAsync<TextAsset>(_asset_name);
            if (map_file == null)
                return (null, null, null);

            var map_data = JsonUtility.FromJson<MapBakeData>(map_file.text);
            if (map_data == null || map_data.Terrain == null)
                return (null, null, null);

            var map_setting = await AssetManager.Instance.LoadAssetAsync<sheet_map_setting>(AssetName.DEMO_MAP_SETTING);
            if (map_setting == null)
            {
                Debug.LogError("Failed to load map setting");
                return (null, null, null);
            }

            var faction_setting = await AssetManager.Instance.LoadAssetAsync<sheet_map_faction_setting>(AssetName.DEMO_MAP_FACTION_SETTING);
            if (faction_setting == null)
            {
                Debug.LogError("Failed to load faction setting");
                return (null, null, null);
            }

            return (map_data, map_setting, faction_setting);        
        }


        void Test_BattleSystem_Setup_Terrain(MapBakeData _map_data)
        {
            if (_map_data == null)
                return;
            
            var terrain_map = new TerrainMap();
            terrain_map.Initialize(_map_data.Terrain.m_width, _map_data.Terrain.m_height);

            //  MASK 값 셋팅.
            for(int y = 0; y < terrain_map.Height; ++y)
            {
                for(int x = 0; x < terrain_map.Width; ++x)
                {
                    terrain_map.Attribute.SetAttributeMask_Static(x, y, _map_data.Terrain.GetAttributeMask_Static(x, y));

                    terrain_map.Attribute.SetCellData(x, y, _map_data.Terrain.GetAttributeMask_Result(x, y));
                }
            }

            // 카운트 셋팅.
            foreach(var e in _map_data.Terrain.m_attribute_dynamic)
            {
                var x = e.m_x;
                var y = e.m_y;
                foreach (var (attribute, count) in e.m_attribute_count)
                {
                    terrain_map.Attribute.IncreaseCellAttributeCount(x, y, (EnumTerrainAttribute)attribute, count);
                }
            }

            // 지형 맵 셋팅
            TerrainMapManager.Instance.SetTerrainMap(terrain_map);

            // 공간 분할 초기화.
            SpacePartitionManager.Instance.Initialize(terrain_map.Width, terrain_map.Height);

            // 카메라 매니저 인스턴스 생성.
            var cam_manager = CameraMananger.Instance;
        }


        void Test_BattleSystem_Setup_Unit(MapBakeData _map_data, sheet_map_setting _map_setting)
        {
            if (_map_data == null || _map_setting == null)
                return;
           

            foreach(var e in _map_data.Entities.m_entities)
            {
                var entity = Entity.Create(e.m_entity_id, e.m_is_fixed_object);    
                if (entity == null)
                {
                    Debug.LogError($"Failed to create entity {e.m_entity_id}");
                    continue;
                }

                // 엔티티 리포지토리에 추가.
                EntityManager.Instance.AddEntity(entity);

                

                // TODO: 이것은 임시로 해둔 코드. 나중에 고정오브젝트를 어떻게 할것인지 생각해보자.
                // FIXEDOBJECT와 연결이 필요할 것이다.
                if (entity.IsFixedObject)
                {
                    // 위치 셋팅.
                    entity.UpdateCellPosition((e.m_cell_x, e.m_cell_y), (_apply: true, _immediatly: true), _is_plan: false);

                    continue;
                }


                var setting_entity = _map_setting.GetEntity(e.m_entity_id);
                var setting_status = _map_setting.GetStatus_EntityID(e.m_entity_id);
                var setting_items  = _map_setting.GetItem_EntityID(e.m_entity_id);
                var ai_type        = _map_setting.GetAIType_EntityID(e.m_entity_id);
                
                if (setting_entity == null || setting_status == null || setting_items == null)
                {
                    Debug.LogError($"Failed to get setting entity {e.m_entity_id}, entity: {setting_entity != null}, status: {setting_status != null}, items: {setting_items != null}");                    
                    continue;
                }

                // 클래스 셋팅.
                entity.StatusManager.Status.SetClassKIND(setting_status.CLASS);


                // 에셋 셋팅.
                entity.SetAssetName(DataManager.Instance.UnitSheet.GetUnitAssetName(entity.StatusManager.Status.ClassKIND, entity.ID));


                // 진영셋팅
                entity.SetFaction(setting_entity.FACTION);

                // AI 타입 셋팅.
                entity.SetAIType(ai_type);

                // 능력치 셋팅.
                entity.StatusManager.Status.SetPoint(EnumUnitPoint.HP,            setting_status.HP);
                entity.StatusManager.Status.SetPoint(EnumUnitPoint.HP_Max,        setting_status.HP);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Level,       setting_status.LEVEL);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Strength,    setting_status.STRENGTH);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Magic,       setting_status.MAGIC);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Skill,       setting_status.SKILL);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Speed,       setting_status.SPEED);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Movement,    setting_status.MOVEMENT);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Defense,     setting_status.DEFENSE);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Proficiency, setting_status.PROFICIENCY);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Resistance,  setting_status.RESISTANCE);
                entity.StatusManager.Status.SetStatus(EnumUnitStatus.Luck,        setting_status.LUCK);


                // 아이템 셋팅.
                foreach(var item in setting_items)
                {
                    var item_entity = Item.Create(Util.GenerateID(), item.ITEM_KIND);
                    entity.Inventory.AddItem(item_entity);
                }    

                // 무기 장착.
                entity.Equip_Weapon_Auto();           

                // 위치 셋팅.
                entity.UpdateCellPosition((e.m_cell_x, e.m_cell_y), (_apply: true, _immediatly: true), _is_plan: false);



                // 유닛 에셋 셋팅.
                // entity.SetAssetName(setting_asset.ASSET_KEY);

                // 월드 오브젝트 생성 처리.
                entity.CreateProcess();
            }

        }

        void Test_BattleSystem_Setup_Faction(sheet_map_faction_setting _faction_setting)
        {
            if (_faction_setting == null)
                return;

            if (_faction_setting.FactionSettings == null)
                return;

            if (_faction_setting.AllianceSettings == null)
                return;

            foreach(var e in _faction_setting.FactionSettings)
            {
                BattleSystemManager.Instance.SetFactionCommanderType(e.Faction, e.CommanderType);
            }

            foreach(var e in _faction_setting.AllianceSettings)
            {
                BattleSystemManager.Instance.SetFactionAlliance(e.Faction_1, e.Faction_2);
            }
        }

        void Test_BattleSystem_Setup_Tag(sheet_map_setting _map_setting)
        {
            if (_map_setting == null)
                return;

            foreach(var e in _map_setting.tag)
            {         
                var tag_data        = TAG_DATA.Create(
                    TAG_INFO.Create((EnumTagType)e.OWNER_TYPE, e.OWNER_VALUE), 
                    (EnumTagAttributeType)e.ATTRIBUTE, 
                    TAG_INFO.Create((EnumTagType)e.TARGET_TYPE, e.TARGET_VALUE));

                TagManager.Instance.SetTag(tag_data);
            }
        }
    }
}