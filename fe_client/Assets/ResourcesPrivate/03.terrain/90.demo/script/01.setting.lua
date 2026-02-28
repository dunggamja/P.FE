-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_01"
SCRIPT_MODULE_FUNC = "Run"

demo_script_01 = {}


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

   -- 유닛(해적3031)_타겟팅포함_진영(선박)
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



-- main 함수.
function demo_script_01.Run()     
   -- 초기 설정 (태그 등).
   demo_script_01.Setting()
end