-- LOAD_AND_RUN이 필요할경우 아래 변수를 설정해야 한다.
SCRIPT_MODULE      = "demo_script_02"
SCRIPT_MODULE_FUNC = "Run"

demo_script_02 = {}

-- lua의 주석
function demo_script_02.Run()  

   local portrait_empty = dialogue.PORTRAIT("", "", "");
   local portrait_jade  = dialogue.PORTRAIT("Jade", "", "");
   local portrait_jax   = dialogue.PORTRAIT("Jax", "", "");
   local portrait_garan = dialogue.PORTRAIT("Garan", "", "");


   -- 유닛들 원래 좌표 받아옴.
   local list_entity_position = EntityManager.GetPosition(tag.TAG_INFO(EnumTagType.Entity_Faction, 1));
   local fix_pos_x, fix_pos_y = 3, 24;
   local list_unit_init_data  = {};
   local list_unit_move_data  = {};


   for i, pos in ipairs(list_entity_position) do
      -- 유닛들 3,24 위치로 이동시켜 놓기.
      table.insert(list_unit_init_data, cutscene.UNIT_MOVE_DATA(pos.id, fix_pos_x, fix_pos_y, fix_pos_x, fix_pos_y));
      
      -- 유닛들 3,24 에서 원래 위치로 이동.
      table.insert(list_unit_move_data, cutscene.UNIT_MOVE_DATA(pos.id, fix_pos_x, fix_pos_y, pos.x, pos.y));
   end


   CutsceneBuilder.RootBegin("Intro");
      -- 전투 시작시 실행.
      CutsceneBuilder.PlayEvent(EnumCutscenePlayEvent.OnBattleStart, 0, 0);

      -- 전투내에서만 사용되는 컷씬.
      CutsceneBuilder.LifeTime(cutscene.LIFE_TIME_BATTLE(false));
      
      -- 트랙 0 
      CutsceneBuilder.TrackBegin();
         -- 유닛 표시 OFF, 
         CutsceneBuilder.Unit_Show({1, 2, 3, 4, 5}, false);
         -- 유닛들 3,24 위치로 이동시켜 놓기.
         CutsceneBuilder.Unit_Move(list_unit_init_data,false);
         -- 로컬 트리거 1번 셋팅. 
         CutsceneBuilder.LocalTriggerSet(1);
      CutsceneBuilder.TrackEnd();

      -- 트랙 1 : 시작.
      CutsceneBuilder.TrackBegin();

         -- 서장 표시.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE({dialogue.CENTER_SHOW(portrait_empty, "서장-demo test")}));
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE_END());

         -- 시작 위치 표시 및 카메라 포커스 (index:0, 성 위치.)
         CutsceneBuilder.VFX_TileSelect_On(0, tag.POSITION(3, 24));         
         CutsceneBuilder.Delay(1.0);

         -- 성 이라는 표시.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.CENTER_SHOW(portrait_empty, "여기가 왕국 성임.")
            }
         ));
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE_END());

         -- 떠나기 전에 대화.
         CutsceneBuilder.Dialogue(dialogue.SEQUENCE(
            {
               dialogue.TOP_SHOW(portrait_jax, 
[[너는 도망가라]]),
               dialogue.TOP_SHOW(portrait_jax, 
[[항구에 배가 준비되있다
빨리가라]]),
               dialogue.BOTTOM_SHOW(portrait_jade, 
[[나는 도망갈거임]]),
               dialogue.BOTTOM_SHOW(portrait_jade, 
[[형만 혼자 남기고
나만 떠날 거임]]),
               dialogue.TOP_SHOW(portrait_jax, 
[[나도 따라갈거임
너 먼저 가서 기다리고 있으면 됨]]),


               dialogue.BOTTOM_SHOW(portrait_jade, 
[[안 가고 같이 싸울까 말까]]),
               dialogue.BOTTOM_SHOW(portrait_jade, 
[[음...]]),
               dialogue.CENTER_SHOW(portrait_garan, 
[[꾸물꾸물대지말고 빨리가자
배 떠나겠다.]]),
               
               dialogue.BOTTOM_HIDE(),
               dialogue.TOP_HIDE(),

               dialogue.CENTER_SHOW(portrait_garan, 
[[자 빨리 짐 싸고 가자
배타러 가자]]),
            }
         ));
         CutsceneBuilder.DialogueEnd();


         -- 타일 선택 해제. (index:0)
         CutsceneBuilder.VFX_TileSelect_Off(0);  


         -- 로컬 트리거 1번 대기.
         CutsceneBuilder.LocalTriggerWait(1);

         -- 유닛들 표시 ON.
         CutsceneBuilder.Unit_Show({1, 2, 3, 4, 5}, true);

         -- 유닛들 3,24 에서 원래 위치로 이동.
         CutsceneBuilder.Unit_Move(list_unit_move_data, true);

         -- 1초 대기.
         CutsceneBuilder.Delay(1.0);


         -- 탈출 위치 표시 및 카메라 포커스 (index:0, 성 위치.)
         CutsceneBuilder.VFX_TileSelect_On(0, tag.POSITION(16, 2));         
         CutsceneBuilder.Delay(1.0);

         -- 여기가 탈출 위치임.
         CutsceneBuilder.Dialogue(
            dialogue.SEQUENCE({dialogue.CENTER_SHOW(portrait_empty, "여기가 탈출 위치임.")})
         );
         CutsceneBuilder.DialogueEnd();

         -- 타일 선택 해제. (index:0)
         CutsceneBuilder.Delay(1.0);
         CutsceneBuilder.VFX_TileSelect_Off(0);  

         -- 시작 위치 표시 및 카메라 포커스 (index:0, 성 위치.)
         CutsceneBuilder.Grid_Cursor(tag.POSITION(3, 22));             


      CutsceneBuilder.TrackEnd();
   CutsceneBuilder.RootEnd();  
end