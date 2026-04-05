-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_01"
SCRIPT_MODULE_FUNC = "Run"

demo_script_01 = {}



-- main 함수.
function demo_script_01.Run()     
   -- 초기 설정 (태그 등).
   demo_script_01.Setting()

   -- 승리 조건 셋팅.
   demo_script_01.Setting_Victory();

   -- 패배 조건 셋팅.
   demo_script_01.Setting_Defeat();
end


function demo_script_01.Setting()
   
   --  진영(아군)_위치(탈출)_위치(015,001)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity_Faction, 1),
         EnumTagAttributeType.POSITION_EXIT,
         tag.POSITION(15, 1)
      )
   );

   -- 진영(적)_포커싱(제외)_진영(선박)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity_Faction, 2),
         EnumTagAttributeType.TARGET_IGNORE,
         tag.TAG_INFO(EnumTagType.Entity_Faction, 3)
      )
   );

   -- 유닛(해적3031)_타겟팅포함_진영(선박)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity, 3031),
         EnumTagAttributeType.TARGET_CONTAIN,
         tag.TAG_INFO(EnumTagType.Entity_Faction, 3)
      )
   );

   -- 유닛(해적3032)_타겟팅포함_진영(선박)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity, 3032),
         EnumTagAttributeType.TARGET_CONTAIN,
         tag.TAG_INFO(EnumTagType.Entity_Faction, 3)
      )
   );

   -- 적 불량배를 그룹 1로 묶어보자. (3021, 3022)
   for entity_id = 3021, 3022 do
      TagManager.SetTag(
         tag.TAG_DATA(
            tag.TAG_INFO(EnumTagType.Entity_Group, 1),
            EnumTagAttributeType.HIERARCHY,
            tag.TAG_INFO(EnumTagType.Entity, entity_id)
         )
      );
   end

   -- 적(불량배들)의 행동순서를 빠르게 셋팅. 
   EntityManager.SetCommandPriority(tag.TAG_INFO(EnumTagType.Entity_Group, 1), 10);

end



-- 승리 조건 셋팅 : 아군전체가 모두 탈출.
function demo_script_01.Setting_Victory()

   -- 아군 전체 탈출 조건 셋팅.
   CutsceneBuilder.RootBegin("Victory_All_Exit");
      -- 승리 체크 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnCheckVictory, 0, 0);
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      -- 아군 전체가 탈출했는지 체크.
      CutsceneBuilder.Condition_Entity(tag.TAG_INFO(EnumTagType.Entity_Faction, 1), EnumEntityCondition.Exit);

      -- 승리 체크 성공시 실행할 로직.
      CutsceneBuilder.TrackBegin();
         -- 승리 처리.
         
      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();
end


-- 패배 조건 셋팅 : 아군전체가 모두 죽음.
function demo_script_01.Setting_Defeat()

   -- 아군 중 1명이라도 사망했는지 체크.
   CutsceneBuilder.RootBegin("Defeat_Anyone_Dead");
      -- 패배 체크 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnCheckDefeat, 0, 0);
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      -- 아군 중 1명이라도 살아있지 않다면 패배.
      CutsceneBuilder.Decorator_Not();
      CutsceneBuilder.Condition_Entity(tag.TAG_INFO(EnumTagType.Entity_Faction, 1), EnumEntityCondition.Alive);

      -- 패배 체크 성공시 실행할 로직.
      CutsceneBuilder.TrackBegin();            
      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();


   -- 상선들이 모두 이탈하거나, 파괴되었으면 패배.
   CutsceneBuilder.RootBegin("Defeat_Ship_Destroyed");
      -- 패배 체크 시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnCheckDefeat, 0, 0);

      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));

      -- 상선들이 모두 맵에 존재하지 않는다면 패배.
      CutsceneBuilder.Decorator_Not();
      CutsceneBuilder.Condition_Entity(tag.TAG_INFO(EnumTagType.Entity_Faction, 3), EnumEntityCondition.Active);

      -- 패배 체크 성공시 실행할 로직.
      CutsceneBuilder.TrackBegin();
      CutsceneBuilder.TrackEnd();

   CutsceneBuilder.RootEnd();
end