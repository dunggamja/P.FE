using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle
{

    public class BattleSystemUpdator : SingletonMono<BattleSystemUpdator>
    {
        protected override float LoopInterval => Constants.BATTLE_SYSTEM_UPDATE_INTERVAL;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // 프로파일링을 위해 임시 처리
            // Debug.unityLogger.logEnabled = false;

            // 지형 시스템 초기화
            Test_BattleSystem_Setup_Terrain();

            // 유닛 생성 & 능력치 셋팅.
            Test_BattleSystem_Setup_Unit();
        }

        protected override void OnLoop()
        {
            base.OnLoop();

            BattleSystemManager.Instance.Update();
        }

        // // Start is called before the first frame update
        // protected override void Start()
        // {
        //     base.Start();
        // }


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
            var attacker_id = Util.GenerateID();
            var defender_id = Util.GenerateID();
            var attacker    = Entity.Create(attacker_id);
            var defender    = Entity.Create(defender_id);


            EntityManager.Instance.AddEntity(attacker);
            EntityManager.Instance.AddEntity(defender);

            attacker.UpdateCellPosition(0, 0, true);
            defender.UpdateCellPosition(2, 3, true);

            attacker.SetFaction(1);
            defender.SetFaction(2);

            // 1. 플레이어 진영, 2: AI 진영.
            //BattleSystemManager.Instance.SetFactionCommanderType(1, EnumCommanderType.Player);
            //BattleSystemManager.Instance.SetFactionCommanderType(2, EnumCommanderType.AI);



            // 테스트를 위한 임시 능력치 셋팅.
            {
                var attacker_status = attacker.StatusManager.Status as UnitStatus;
                var defender_status = defender.StatusManager.Status as UnitStatus;

                // var attacker_weapon = attacker.StatusManager.Weapon as Weapon;
                // var defender_weapon = defender.StatusManager.Weapon as Weapon;

                attacker_status.SetPoint(EnumUnitPoint.HP, 22);
                attacker_status.SetStatus(EnumUnitStatus.Strength, 6);
                attacker_status.SetStatus(EnumUnitStatus.Skill, 5);
                attacker_status.SetStatus(EnumUnitStatus.Speed, 7);
                attacker_status.SetStatus(EnumUnitStatus.Movement, 4);
                attacker_status.SetStatus(EnumUnitStatus.Defense, 5);
                attacker_status.SetStatus(EnumUnitStatus.Resistance, 3);
                attacker_status.SetStatus(EnumUnitStatus.Luck, 5);
                attacker_status.SetStatus(EnumUnitStatus.Weight, 4);


                defender_status.SetPoint(EnumUnitPoint.HP, 22);
                defender_status.SetStatus(EnumUnitStatus.Strength, 6);
                defender_status.SetStatus(EnumUnitStatus.Skill, 5);
                defender_status.SetStatus(EnumUnitStatus.Speed, 7);
                defender_status.SetStatus(EnumUnitStatus.Movement, 4);
                defender_status.SetStatus(EnumUnitStatus.Defense, 5);
                defender_status.SetStatus(EnumUnitStatus.Resistance, 3);
                defender_status.SetStatus(EnumUnitStatus.Luck, 5);
                defender_status.SetStatus(EnumUnitStatus.Weight, 4);


                attacker.SetPathAttribute(EnumPathOwnerAttribute.Ground);
                defender.SetPathAttribute(EnumPathOwnerAttribute.Ground);
            }


            // 무기 셋팅.
            {
                var attacker_weapon = new Item(Util.GenerateID(), 0);
                var defender_weapon = new Item(Util.GenerateID(), 0);
                attacker_weapon.SetWeaponStatus(EnumWeaponStatus.Might_Physics, 3);
                attacker_weapon.SetWeaponStatus(EnumWeaponStatus.Hit, 100);
                attacker_weapon.SetWeaponStatus(EnumWeaponStatus.Weight, 2);
                attacker_weapon.SetWeaponStatus(EnumWeaponStatus.Dodge_Critical, 30);
                attacker_weapon.SetWeaponStatus(EnumWeaponStatus.Range, 1);
                attacker_weapon.SetWeaponStatus(EnumWeaponStatus.Range_Min, 1);

                defender_weapon.SetWeaponStatus(EnumWeaponStatus.Might_Physics, 3);
                defender_weapon.SetWeaponStatus(EnumWeaponStatus.Hit, 100);
                defender_weapon.SetWeaponStatus(EnumWeaponStatus.Weight, 2);
                defender_weapon.SetWeaponStatus(EnumWeaponStatus.Dodge_Critical, 30);
                defender_weapon.SetWeaponStatus(EnumWeaponStatus.Range, 1);
                defender_weapon.SetWeaponStatus(EnumWeaponStatus.Range_Min, 1);

                attacker.Inventory.AddItem(attacker_weapon);
                defender.Inventory.AddItem(defender_weapon);    

                attacker.StatusManager.Weapon.Equip(attacker_weapon.ID);
                defender.StatusManager.Weapon.Equip(defender_weapon.ID);
            }


            WorldObjectManager.Instance.CreateObject(attacker.ID);
            WorldObjectManager.Instance.CreateObject(defender.ID);
        }



    }
}