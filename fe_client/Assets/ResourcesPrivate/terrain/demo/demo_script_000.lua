SCRIPT_MODULE      = "demo_script_000"
SCRIPT_MODULE_FUNC = "Run"

demo_script_000 = {}

function demo_script_000.Run()     
   
   --  진영(아군)_위치(탈출)_위치(015,001)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity_Faction, 1),
         EnumTagAttributeType.POSITION_EXIT,
         tag.TAG_POSITION(15, 1)
      )
   )

   -- 진영(적)_포커싱(제외)_진영(선박)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity_Faction, 2),
         EnumTagAttributeType.TARGET_IGNORE,
         tag.TAG_INFO(EnumTagType.Entity_Faction, 3)
      )
   )

   -- 유닛(해적3031)_타겟팅포함_진영(선박)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity, 3031),
         EnumTagAttributeType.TARGET_CONTAIN,
         tag.TAG_INFO(EnumTagType.Entity_Faction, 3)
      )
   )

   -- 유닛(해적3031)_타겟팅포함_진영(선박)
   TagManager.SetTag(
      tag.TAG_DATA(
         tag.TAG_INFO(EnumTagType.Entity, 3032),
         EnumTagAttributeType.TARGET_CONTAIN,
         tag.TAG_INFO(EnumTagType.Entity_Faction, 3)
      )
   )
end